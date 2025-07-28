import pyodbc
from contextlib import contextmanager
from typing import Any, List
from sqlalchemy import create_engine

from langchain.tools import tool
from langchain_core.messages import ToolMessage
from langchain_core.messages.tool import ToolCall
from langchain_core.tools import BaseTool

from runeterra.config import Config
from runeterra.logger import log, log_panel

engine = create_engine(
    "mssql+pyodbc://sa:123@localhost:1433/FigureManagementSystem?driver=ODBC+Driver+17+for+SQL+Server",
    pool_size=10,     
    max_overflow=5,    
    pool_timeout=30,  
    pool_recycle=1800,  
)

def get_connection():
    return engine.raw_connection()


def get_available_tools() -> List[BaseTool]:
    return [
        execute_sql,
        list_tables,
        describe_table,
        sample_table
    ]

def call_tool(tool_call: ToolCall) -> Any:
    tools_by_name = {tool.name: tool for tool in get_available_tools()}
    tool = tools_by_name[tool_call["name"]]
    response = tool.invoke(tool_call["args"])
    return ToolMessage(content=response, tool_call_id=tool_call["id"])


@contextmanager
def with_sql_cursor(readonly=True):
    conn = get_connection()
    cur = conn.cursor()
    try:
        yield cur
        if not readonly:
            conn.commit()
    except Exception:
        if not readonly:
            conn.rollback()
        raise
    finally:
        cur.close()
        conn.close()

def _execute_sql(reasoning: str, sql_query: str) -> str:
    log_panel(
        title="Execute SQL Tool",
        content=f"Query: {sql_query}\nReasoning: {reasoning}",
    )
    try:
        with with_sql_cursor() as cursor:
            cursor.execute(sql_query)
            rows = cursor.fetchall()
        return "\n".join([str(row) for row in rows])
    except Exception as e:
        log(f"[red]Error running query: {str(e)}[/red]")
        return f"Error running query: {str(e)}"
    pass

@tool(parse_docstring=True)
def execute_sql(reasoning: str, sql_query: str) -> str:
    """Executes a SQL query on the MySQL database and returns the result.
    PERFORMANCE GUIDELINES:
    - Use LIMIT clause for large result sets
    - Prefer specific columns over SELECT *
    - Use appropriate WHERE clauses with indexed columns
    - Consider using specialized tools for common queries

    Args:
        sql_query: A complete, valid SQL Server SQL query.

    Returns:
        Query result as a string, showing each row as a tuple on a new line.
    """
    return _execute_sql(reasoning, sql_query)

@tool(parse_docstring=True)
def list_tables(reasoning: str) -> str:
    """
    Lists all user-created tables in the database (excludes SQL Server system tables).
    
    Args:
        reasoning: Detailed explanation of why you need to see all tables (relate to the user's query).
    
    Returns:
        String representation of a list containing all table names, schema-qualified.
    """
    log_panel(
        title="List Tables Tool",
        content=f"Reasoning: {reasoning}"
    )
    try:
        with with_sql_cursor() as cursor:
            cursor.execute("""
                SELECT TABLE_SCHEMA, TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'BASE TABLE'
                  AND TABLE_NAME NOT LIKE 'sys%'
                  AND TABLE_NAME NOT LIKE 'MSpeer_%'
            """)
            tables = [f"{row[0]}.{row[1]}" for row in cursor.fetchall()]
            return str(tables)
    except Exception as e:
        log_panel(
            title="Error listing tables",
            content=f"Error: {str(e)}"
        )
        return f"Error listing tables: {str(e)}"


# ====== Tool 2: describe_table ======
@tool(parse_docstring=True)
def describe_table(reasoning: str, table_name: str) -> str:
    """
    Returns detailed schema information about a table (columns, types, constraints).
    
    Args:
        reasoning: Detailed explaination of why you need to understand this table's structure
        table_name: Exact name of the table to describe (case-sensitive, no quote needed)
    
    Returns:
        String showing each column's name, datatype, nullability and size.
    """
    log_panel(
        title="Describe Table Tool",
        content=f"Reasoning: {reasoning}\nTable: {table_name}"
    )
    try:
        with with_sql_cursor() as cursor:
            cursor.execute(
                """
                SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, 
                       COALESCE(CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, '') as SIZE
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = ?
                """, (table_name,)
            )
            rows = cursor.fetchall()
            if not rows:
                return f"No columns found for table '{table_name}'."
            header = "Column | Type | Nullable | Size\n------ | ---- | -------- | ----"
            lines = [
                f"{r[0]} | {r[1]} | {r[2]} | {str(r[3]) if r[3] is not None else ''}"
                for r in rows
            ]
            return "\n".join([header] + lines)
    except Exception as e:
        log_panel(
            title="Error describing table",
            content=f"Table: {table_name}\nError: {str(e)}"
        )
        return f"Error describing table '{table_name}': {str(e)}"


# ====== Tool 3: sample_table ======
@tool(parse_docstring=True)
def sample_table(reasoning: str, table_name: str, row_sample_size: int = 5) -> str:
    """
    Returns a sample of rows from the specified table.

    Args:
        reasoning: Detailed explaination of why you need to see example data from this table
        table_name: Exact name of the table to sample (case-sensitive, no quotes needed)
        row_sample_size: Number of rows retrieves (recommended: 3-5 rows for readability)
    
    Returns:
        String with one row per line, showing all columns for each row as tuples
    """
    log_panel(
        title="Sample Table Tool",
        content=f"Table: {table_name}\nRows: {row_sample_size}\nReasoning: {reasoning}"
    )
    try:
        with with_sql_cursor() as cursor:
            cursor.execute(f"SELECT TOP {row_sample_size} * FROM {table_name}")
            rows = cursor.fetchall()
            if not rows:
                return f"No data found in table '{table_name}'."
            return "\n".join([str(row) for row in rows])
    except Exception as e:
        log_panel(
            title="Error sampling table",
            content=f"Table: {table_name}\nError: {str(e)}"
        )
        return f"Error sampling from table '{table_name}': {str(e)}"


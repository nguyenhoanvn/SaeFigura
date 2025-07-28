import os
from dataclasses import dataclass
from enum import Enum

class ModelProvider(str, Enum):
    OLLAMA = "ollama"
    GROQ = "groq"

@dataclass
class ModelConfig:
    name: str
    temperature: float
    provider: ModelProvider


LLAMA_3 = ModelConfig("llama3", 0.0, ModelProvider.OLLAMA)
QWEN_3 = ModelConfig("qwen/qwen3-32b", 0.0, ModelProvider.GROQ)

class Config:
    MODEL = QWEN_3
    OLLAMA_CONTEXT_WINDOW = 2048
    
    class SQLServer:
        HOST = os.getenv("MYSQL_HOST", "localhost")
        PORT = int(os.getenv("MYSQL_PORT", 1433))
        USER = os.getenv("MYSQL_USER", "sa")
        PASSWORD = os.getenv("SQLSERVER_PASSWORD", "123")
        DATABASE = os.getenv("SQLSERVER_DATABASE", "FigureManagementSystem")
        DRIVER = os.getenv("SQLSERVER_DRIVER", "ODBC Driver 17 for SQL Server")

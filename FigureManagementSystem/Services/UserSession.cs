using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FigureManagementSystem.Models;
using Microsoft.VisualBasic.ApplicationServices;

namespace FigureManagementSystem.Services
{
    public class UserSession
    {
        private static UserSession _instance;
        private static readonly object _lock = new object();

        public static UserSession Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new UserSession();
                }
            }
        }

        public TblUser CurrentUser { get; set; }

        private UserSession() { }
    }

}

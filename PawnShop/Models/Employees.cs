using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace PawnShop.Models
{
    public class Employees : INotifyPropertyChanged
    {
        private int employeeID;
        private string fullName = string.Empty;
        private string position;
        private DateTime hireDate = DateTime.Now;
        private string login = string.Empty;
        private string passwordHash = string.Empty;
        private string role = "Employee";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Key]
        public int EmployeeID
        {
            get => employeeID;
            set
            {
                if (employeeID != value)
                {
                    employeeID = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FullName
        {
            get => fullName;
            set
            {
                if (fullName != value)
                {
                    fullName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Position
        {
            get => position;
            set
            {
                if (position != value)
                {
                    position = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime HireDate
        {
            get => hireDate;
            set
            {
                if (hireDate != value)
                {
                    hireDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Login
        {
            get => login;
            set
            {
                if (login != value)
                {
                    login = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PasswordHash
        {
            get => passwordHash;
            set
            {
                if (passwordHash != value)
                {
                    passwordHash = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Role
        {
            get => role;
            set
            {
                if (role != value)
                {
                    role = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
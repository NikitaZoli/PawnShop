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
    }
}
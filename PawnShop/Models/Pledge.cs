using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PawnShop.Models
{
    public class Pledge : INotifyPropertyChanged
    {
        private int pledgeID;
        private int clientID;
        private string itemDescription;
        private decimal estimatedValue;
        private decimal loanAmount;
        private decimal interestRate;
        private DateTime pledgeDate;
        private DateTime dueDate;
        private string status;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int PledgeID
        {
            get => pledgeID;
            set
            {
                if (pledgeID != value)
                {
                    pledgeID = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ClientID
        {
            get => clientID;
            set
            {
                if (clientID != value)
                {
                    clientID = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ItemDescription
        {
            get => itemDescription;
            set
            {
                if (itemDescription != value)
                {
                    itemDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal EstimatedValue
        {
            get => estimatedValue;
            set
            {
                if (estimatedValue != value)
                {
                    estimatedValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal LoanAmount
        {
            get => loanAmount;
            set
            {
                if (loanAmount != value)
                {
                    loanAmount = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal InterestRate
        {
            get => interestRate;
            set
            {
                if (interestRate != value)
                {
                    interestRate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime PledgeDate
        {
            get => pledgeDate;
            set
            {
                if (pledgeDate != value)
                {
                    pledgeDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime DueDate
        {
            get => dueDate;
            set
            {
                if (dueDate != value)
                {
                    dueDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
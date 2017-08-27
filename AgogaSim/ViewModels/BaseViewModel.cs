using System.Collections.Generic;
using System.ComponentModel;

namespace AgogaSim
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Notify(IList<string> propertyNames)
        {
            if (PropertyChanged != null)
                foreach (var propertyName in propertyNames)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        bool isProcessing;
        public bool IsProcessing
        {
            get { return isProcessing; }
            set
            {
                isProcessing = value;
                Notify("IsNotProcessing");
                Notify("IsProcessing");
            }
        }

        public bool IsNotProcessing
        {
            get { return !IsProcessing; }
        }
    }
}

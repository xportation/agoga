using System.Collections.Generic;
using System.ComponentModel;

namespace AgogaSim
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
                IsEnabled = !isProcessing;

                Notify("IsProcessing");
            }
        }

        bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                Notify("IsEnabled");
            }
        }
    }
}

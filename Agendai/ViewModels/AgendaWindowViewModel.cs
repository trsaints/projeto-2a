using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Agendai.ViewModels
{
    public class AgendaWindowViewModel : ViewModelBase
    {
        public string Title { get; set; } = "Agenda";

        public string[] Months { get; set; } = new string[]
        {
            "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", 
            "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
        };

        public string[] Days { get; set; } = new string[]
        {
            "Domingo", "Segunda-feira", "Terça-feira", "Quarta-feira", "Quinta-feira", 
            "Sexta-feira", "Sábado"
        };

        public string[] Hours { get; set; } = new string[]
        {
            "00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00", 
            "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", 
            "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00"
        };

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                SetProperty(ref _selectedIndex, value);
                UpdateDataGridItems();
            }
        }

        private ObservableCollection<string> _dataGridItems;
        public ObservableCollection<string> DataGridItems
        {
            get => _dataGridItems;
            set => SetProperty(ref _dataGridItems, value);
        }

        public AgendaWindowViewModel()
        {
            DataGridItems = new ObservableCollection<string>(Months);
        }

        private void UpdateDataGridItems()
        {
            switch (_selectedIndex)
            {
                case 0: 
                    DataGridItems = new ObservableCollection<string>(Months);
                    break;
                case 1: 
                    DataGridItems = new ObservableCollection<string>(Days);
                    break;
                case 2: 
                    DataGridItems = new ObservableCollection<string>(Hours);
                    break;
                default:
                    DataGridItems = new ObservableCollection<string>();
                    break;
            }
        }
    }
}

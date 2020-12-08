using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Example
{
    public partial class MainPage : ContentPage, ICropper
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel(this);
        }

        public async Task<Stream> Crop()
        {
            return await cropper.GetImageAsJpegAsync(90, 200, 200);
        }
    }

    public interface ICropper
    {
        Task<Stream> Crop();
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand CropCommand { get; }

        private ImageSource resultPhotoSource { get; set; }
        public ImageSource ResultPhotoSource
        {
            get => resultPhotoSource;
            set
            {
                resultPhotoSource = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(ICropper cropper)
        {
            CropCommand = new Command(async () =>
            {
                var cropped = await cropper.Crop();
                ResultPhotoSource = ImageSource.FromStream(() => cropped);
            });
        }
    }
}

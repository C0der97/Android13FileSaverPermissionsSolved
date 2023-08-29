using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using System.Text;
using System.Threading;

#if ANDROID
using Android;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;
#endif


namespace PermisosMaui
{
    public partial class MainPage : ContentPage
    {
        int count = 0;


        public MainPage()
        {
            _ = Permissions.RequestAsync<Permissions.StorageRead>();
            _ = Permissions.RequestAsync<Permissions.StorageWrite>();
            _ = Permissions.RequestAsync<Permissions.Camera>();
            _ = Permissions.RequestAsync<Permissions.Photos>();
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            CancellationToken ctk = CancellationToken.None;

            try
            {
                await Permissions.RequestAsync<Permissions.Camera>();
                //await Permissions.RequestAsync<Permissions.Photos>();
                //await Permissions.RequestAsync<Permissions.Media>();
                //await Permissions.RequestAsync<Permissions.StorageRead>();
                //await Permissions.RequestAsync<Permissions.StorageWrite>();




                if (DeviceInfo.Platform == DevicePlatform.Android && OperatingSystem.IsAndroidVersionAtLeast(33))
                {
#if ANDROID
                    var activity = Platform.CurrentActivity ?? throw new NullReferenceException("Current activity is null");
                    if (ContextCompat.CheckSelfPermission(activity, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                    {
                        ActivityCompat.RequestPermissions(activity, new[] { Manifest.Permission.ReadExternalStorage }, 1);
                    }
#endif
                }
                else
                {
                    var status = await Permissions.RequestAsync<Permissions.StorageWrite>().WaitAsync(ctk).ConfigureAwait(false);

                    if (status is not PermissionStatus.Granted)
                    {
                        throw new PermissionException("Storage permission is not granted.");
                    }
                }

                using var stream = new MemoryStream(Encoding.Default.GetBytes("Hello from the Community Toolkit!"));
                var fileSaverResult = await FileSaver.SaveAsync("test.txt", stream, ctk);
                fileSaverResult.EnsureSuccess();
                await Toast.Make($"File is saved: {fileSaverResult.FilePath}").Show(ctk);

                count++;

                if (count == 1)
                    CounterBtn.Text = $"Clicked {count} time";
                else
                    CounterBtn.Text = $"Clicked {count} times";

                SemanticScreenReader.Announce(CounterBtn.Text);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
            }


        }
    }
}
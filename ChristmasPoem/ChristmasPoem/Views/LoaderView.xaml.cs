namespace ChristmasPoem.Views;

public partial class LoaderView : ContentView
{

    public LoaderView()
    {
        InitializeComponent();




        MainThread.BeginInvokeOnMainThread(() =>
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(0.0333));
            TurnArround();
            Task.Run(async () =>
            {
                while (await timer.WaitForNextTickAsync())
                {
                    TurnArround();
                }
            });
        });

    }
    public void TurnArround()
    {
        SpinnerView.Rotation = SpinnerView.Rotation + 10;
    }
}

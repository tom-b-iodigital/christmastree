using ChristmasPoem.ViewModels;

namespace ChristmasPoem;

public partial class MainPage : ContentPage
{

	public MainPage(MainPageViewModel mainPageViewModel)
	{
		InitializeComponent();

		BindingContext = mainPageViewModel;
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
	}
}


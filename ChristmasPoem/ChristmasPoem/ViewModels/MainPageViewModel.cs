using ChristmasPoem.Messages;
using ChristmasPoem.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace ChristmasPoem.ViewModels
{
    [ObservableObject]
    public partial class MainPageViewModel
    {
        private readonly IAIService _aIService;

        [ObservableProperty]
        private string _keyword = "marketing";

        [ObservableProperty]
        private string _poem;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PoemStateString))]
        private PoemStatus _poemState = PoemStatus.NoPoem;

        public string PoemStateString => _poemState.ToString();

        public MainPageViewModel(IAIService aIService)
        {
            _aIService = aIService;

            WeakReferenceMessenger.Default.Register<WordSaidMessage>(this, (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Keyword = message.Value;

                    await GenerateNewPoem();
                });
            });

        }

        partial void OnPoemStateChanged(PoemStatus value)
        {
            WeakReferenceMessenger.Default.Send<RecitingPoemStatusMessage>(new(PoemState));
        }

        [RelayCommand]
        public async Task GenerateNewPoem()
        {
            Poem = string.Empty;
            PoemState = PoemStatus.FetchingPoem;
            Poem = await _aIService.GetPoemAsync(Keyword);

            if (!string.IsNullOrEmpty(Poem))
            {

                try
                {
                    PoemState = PoemStatus.RecitingPoem;
                    await TextToSpeech.Default.SpeakAsync(Poem);
                    PoemState = PoemStatus.DisplayingPoem;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        
    }
}

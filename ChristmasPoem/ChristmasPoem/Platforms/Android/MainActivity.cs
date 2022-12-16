using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Widget;
using AndroidX.Core.App;
using ChristmasPoem.Messages;
using ChristmasPoem.Services.Implementations;
using CommunityToolkit.Mvvm.Messaging;

namespace ChristmasPoem;

[Activity(Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[MetaData("android.app.shortcuts", Resource = "@xml/shortcuts")]
public class MainActivity : MauiAppCompatActivity, IRecognitionListener
{
    private const int ASR_PERMISSION_REQUEST_CODE = 0;
    private const string START_LISTENING_COMMAND = "hello christmas tree";
    SpeechRecognizer _speechRecognizer = null;
    private DateTime _startWordMode;
    private bool _wordMode = false;
    private string _word = "";

    private string _greeting = """
        I'm a Christmas Tree,
        Twinkling in the night,
        Glittering with lights and ornaments so bright.
        So say a word and let's create a poem
        """;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        VerifyAudioPermissions();

        WeakReferenceMessenger.Default.Register<RecitingPoemStatusMessage>(this, (sender, message) =>
        {
            switch (message.Value)
            {
                case PoemStatus.RecitingPoem:
                    StopListening();
                    break;
                case PoemStatus.DisplayingPoem:
                    StartListening();
                    break;
                default:
                    break;
            }
        });

        base.OnCreate(savedInstanceState);
    }

    private async Task RefreshGreeting()
    {
        var aiService = new AIService();

        string newGreeting = await aiService.GetGreetingAsync();

        if (!string.IsNullOrWhiteSpace(newGreeting))
        {
            _greeting = newGreeting;
        }
    }

    private Intent CreateIntent()
    {
        Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
        intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelWebSearch);
        intent.PutExtra(RecognizerIntent.ExtraCallingPackage, PackageName);
        intent.PutExtra(RecognizerIntent.ExtraPartialResults, true);
        intent.PutExtra(RecognizerIntent.ExtraLanguage, "en-US");

        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            intent.PutExtra(RecognizerIntent.ExtraPreferOffline, true);
        }

        return intent;
    }

    private void StartListening()
    {
        if (_speechRecognizer == null)
        {
            _speechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(this);

            _speechRecognizer.SetRecognitionListener(this);
            _speechRecognizer.StartListening(CreateIntent());
        }
    }

    private void StopListening()
    {
        string word = _word;
        if (_wordMode && !string.IsNullOrWhiteSpace(word))
        {
            Task.Run(async () =>
            {
                try
                {
                    await TextToSpeech.Default.SpeakAsync($"{word} is a great choice, give me a moment to write a poem for you");

                    WeakReferenceMessenger.Default.Send<WordSaidMessage>(new(word));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });

            _wordMode = false;
            _word = string.Empty;
        }
        else if (_wordMode)
        {
            // after thirty seconds without catching a word, revert back to hello tree mode
            if ((DateTime.Now - _startWordMode).TotalSeconds > 30)
            {
                _wordMode = false;
                _word = string.Empty;

            }
        }

        if (_speechRecognizer != null)
        {
            _speechRecognizer.Destroy();
            _speechRecognizer = null;
        }
    }

    private void VerifyAudioPermissions()
    {
        if (CheckCallingOrSelfPermission(Manifest.Permission.RecordAudio) != Permission.Granted)
        {
            ActivityCompat.RequestPermissions(
                this,
                new[] { Manifest.Permission.RecordAudio },
                ASR_PERMISSION_REQUEST_CODE
            );
        }
        else
        {
            StartListening();
        }
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        if (requestCode == ASR_PERMISSION_REQUEST_CODE)
        {
            if (grantResults[0] == Permission.Granted)
            {
                // audio permission granted
                Toast.MakeText(this, "You can now use voice commands!", ToastLength.Long).Show();
                StartListening();
            }
        }

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    public void OnBeginningOfSpeech()
    {
        Console.WriteLine("OnBeginningOfSpeech");
    }

    public void OnBufferReceived(byte[] buffer)
    {
    }

    public void OnEndOfSpeech()
    {
        Console.WriteLine("OnEndOfSpeech");

        StopListening();
        StartListening();
    }

    public void OnError([GeneratedEnum] SpeechRecognizerError error)
    {
        if (_wordMode)
        {
            // after thirty seconds without catching a word, revert back to hello tree mode
            if ((DateTime.Now - _startWordMode).TotalSeconds > 30)
            {
                _wordMode = false;
                _word = string.Empty;

            }
        }

        StopListening();
        StartListening();
    }

    public void OnEvent(int eventType, Bundle @params)
    {
    }

    public void OnPartialResults(Bundle partialResults)
    {
        Console.WriteLine("OnPartialResults");

        var matches = partialResults.GetStringArrayList(SpeechRecognizer.ResultsRecognition);

        if (matches != null && matches.Count > 0)
        {
            // The results are added in decreasing order of confidence to the list
            foreach (var match in matches)
            {
                if (!_wordMode)
                {
                    if (match.ToLower() == START_LISTENING_COMMAND)
                    {
                        StopListening();
                        Task.Run(async () =>
                        {
                            try
                            {
                                await TextToSpeech.Default.SpeakAsync(_greeting);

                                _ = RefreshGreeting();

                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    _startWordMode = DateTime.Now;
                                    _wordMode = true;
                                    StartListening();
                                });
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        });
                    }
                }
                else
                {
                    _word = match;
                }
            }
        }
    }

    public void OnReadyForSpeech(Bundle @params)
    {
        Console.WriteLine("OnReadyForSpeech");
    }

    public void OnResults(Bundle results)
    {
    }

    public void OnRmsChanged(float rmsdB)
    {
    }
}

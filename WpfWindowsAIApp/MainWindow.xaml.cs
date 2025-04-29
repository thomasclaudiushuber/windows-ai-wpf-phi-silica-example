using System.Windows;
using Microsoft.Windows.AI;
using Microsoft.Windows.AI.Generative;
using Windows.Foundation;

namespace WpfWindowsAIApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void ButtonGenerate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            btnGenerate.IsEnabled = false;
            txtResponse.Text = "";

            txtStatus.Text = "Loading model into memory...";

            if (LanguageModel.GetReadyState() == AIFeatureReadyState.EnsureNeeded)
            {
                var result = await LanguageModel.EnsureReadyAsync();
                if (result.Status != AIFeatureReadyResultState.Success)
                {
                    throw new Exception(result.ExtendedError.Message);
                }
            }

            using LanguageModel languageModel = await LanguageModel.CreateAsync();

            txtStatus.Text = "Generating answer...";

            string prompt = txtPrompt.Text;

            AsyncOperationProgressHandler<LanguageModelResponseResult, string> handler = async (asyncInfo, str) =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    txtResponse.Text += str;
                });
            };

            var asyncOp = languageModel.GenerateResponseAsync(prompt);
            asyncOp.Progress = handler;
            await asyncOp;
        }
        finally
        {
            btnGenerate.IsEnabled = true;
            txtStatus.Text = "";
        }
    }
}
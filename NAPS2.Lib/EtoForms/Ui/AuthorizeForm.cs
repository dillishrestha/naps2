using System.Threading;
using Eto.Drawing;
using NAPS2.EtoForms.Layout;
using NAPS2.ImportExport.Email.Oauth;

namespace NAPS2.EtoForms.Ui;

public class AuthorizeForm : EtoDialogBase
{
    private readonly ErrorOutput _errorOutput;
    private CancellationTokenSource? _cancelTokenSource;

    public AuthorizeForm(Naps2Config config, ErrorOutput errorOutput) : base(config)
    {
        _errorOutput = errorOutput;
    }

    protected override void BuildLayout()
    {
        Title = UiStrings.AuthorizeFormTitle;
        Icon = new Icon(1f, Icons.key_small.ToEtoImage());

        FormStateController.FixedHeightLayout = true;
        FormStateController.RestoreFormState = false;
        FormStateController.Resizable = false;

        LayoutController.Content = L.Row(
            C.Label(UiStrings.WaitingForAuthorization).Padding(right: 30),
            C.CancelButton(this)
        );
    }

    public OauthProvider? OauthProvider { get; set; }

    public bool Result { get; private set; }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (OauthProvider == null) throw new InvalidOperationException("OauthProvider must be specified");

        _cancelTokenSource = new CancellationTokenSource();
        Task.Run(() =>
        {
            try
            {
                OauthProvider.AcquireToken(_cancelTokenSource.Token);
                Invoker.Current.Invoke(() =>
                {
                    Result = true;
                    Close();
                });
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorOutput.DisplayError(MiscResources.AuthError, ex);
                Log.ErrorException("Error acquiring Oauth token", ex);
                Invoker.Current.Invoke(Close);
            }
        });
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _cancelTokenSource?.Cancel();
    }
}
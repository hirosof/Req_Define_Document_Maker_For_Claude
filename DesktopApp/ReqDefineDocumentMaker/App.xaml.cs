using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace ReqDefineDocumentMaker;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // アプリケーション全体の未処理例外をキャッチ
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show($"エラーが発生しました:\n\n{e.Exception.Message}\n\nスタックトレース:\n{e.Exception.StackTrace}",
            "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        MessageBox.Show($"致命的なエラーが発生しました:\n\n{exception?.Message}\n\nスタックトレース:\n{exception?.StackTrace}",
            "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}


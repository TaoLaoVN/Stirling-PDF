// Prevents additional console window on Windows in release, DO NOT REMOVE!!
#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

fn main() {
  #[cfg(target_os = "windows")]
  {
    if std::env::var_os("WEBVIEW2_BROWSER_EXECUTABLE_FOLDER").is_none() {
      // 1. Check portable webview2 folder alongside executable
      if let Ok(exe_path) = std::env::current_exe() {
        if let Some(exe_dir) = exe_path.parent() {
          let local_wv2 = exe_dir.join("webview2");
          if local_wv2.join("msedgewebview2.exe").exists() {
            std::env::set_var("WEBVIEW2_BROWSER_EXECUTABLE_FOLDER", &local_wv2);
          }
        }
      }

      // 2. Check extracted Fixed Version in C:\WebView2Runtime
      if std::env::var_os("WEBVIEW2_BROWSER_EXECUTABLE_FOLDER").is_none() {
        let fixed_path = std::path::Path::new(r"C:\WebView2Runtime\Microsoft.WebView2.FixedVersionRuntime.155.0.4283.45.x64");
        if fixed_path.join("msedgewebview2.exe").exists() {
          std::env::set_var("WEBVIEW2_BROWSER_EXECUTABLE_FOLDER", fixed_path);
        }
      }

      // 3. Check LocalAppData user installation
      if std::env::var_os("WEBVIEW2_BROWSER_EXECUTABLE_FOLDER").is_none() {
        if let Ok(local_appdata) = std::env::var("LOCALAPPDATA") {
          let user_wv2 = std::path::PathBuf::from(local_appdata)
            .join("Microsoft")
            .join("EdgeWebView")
            .join("Application")
            .join("155.0.4283.45");
          if user_wv2.join("msedgewebview2.exe").exists() {
            std::env::set_var("WEBVIEW2_BROWSER_EXECUTABLE_FOLDER", user_wv2);
          }
        }
      }
    }
  }

  app_lib::run();
}

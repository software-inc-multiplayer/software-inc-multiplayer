const {
  app,
  BrowserWindow,
  screen,
  shell
} = require('electron');
const url = require('url');
const path = require('path');
require('electron-debug')();
const electronLocalshortcut = require('electron-localshortcut');

function createWindow() {
  // Create the browser window.
  const win = new BrowserWindow({
    width: screen.getPrimaryDisplay().size.width / 1.25,
    height: screen.getPrimaryDisplay().size.height / 1.25,
    title: "Software Inc Multiplayer Mod",
    webPreferences: {
      nodeIntegration: true,
      nodeIntegrationInWorker: true
    },
    icon: __dirname + '\\icon.ico'
  })

  process.env.ELECTRON_DISABLE_SECURITY_WARNINGS = true; 
  // win.loadURL(url.format({
  //   pathname: path.join(__dirname, 'pages', 'index.html'),
  //   protocol: 'file:',
  //   slashes: true
  // }));
  win.loadFile(__dirname + "/pages/index.html");

  electronLocalshortcut.register(win, 'CTRL+I', () => {
    win.webContents.openDevTools();
  });
  win.removeMenu();
}

app.whenReady().then(createWindow);
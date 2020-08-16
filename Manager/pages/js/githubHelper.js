const settings = require('electron-settings');
const homedir = require('os').homedir();
const Electron = require('electron').remote
const request = require('request');
async function getReleases() {
    let releases = [];
    await fetch('https://api.github.com/repos/cal3432/software-inc-multiplayer/releases').then(response => response.json()).then(res => {
        const data = res;
        data.forEach(release => {
            let hasBinaries = false;
            let binaries;
            let tiern;
            release.assets.forEach(asset => {
                if (asset.name == "installer-binaries.zip") {
                    hasBinaries = true;
                    binaries = asset.browser_download_url;
                }
            });
            if (!hasBinaries) return;
            let isUntstable = false;
            if (release.tag_name.includes("-unstable")) isUntstable = true;
            if (release.tag_name.includes("-launcher")) return;
            const releaseData = {
                name: release.tag_name,
                unstable: isUntstable,
                releaseDate: release.published_at,
                download: binaries,
                url: release.html_url
            }
            localStorage.setItem(releaseData.name, JSON.stringify(releaseData));
            releases.push(releaseData);
        });
    });
    return releases;
}
async function getInstallationDir() {
    if (await settings.has('installDir')) return await settings.get('installDir');

    let options = {
        properties: ["openDirectory"],
        title: "Select Software Inc Installation Directory",
        defaultPath: homedir,
        buttonLabel: "Select Folder",
    }
    let dir;
    dir = Electron.dialog.showOpenDialogSync(options);
    await settings.set('installDir', dir);
    return dir;
}
module.exports = {
    getInstallationDir,
    getReleases
}
/**
 * 

*/
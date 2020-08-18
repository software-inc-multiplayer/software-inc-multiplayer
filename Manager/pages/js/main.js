const gitHubHelper = require('./js/githubHelper.js');
const path = require('path');
const getPath = require('platform-folders').default;
const fs = require('fs');
const extract = require('extract-zip');
const settings = require('electron-settings');
const stream = require('stream');
const open = require('open');
const {
    promisify
} = require('util');

const pipeline = promisify(stream.pipeline);

const ncp = require('ncp').ncp;
const got = require('got').default;
async function load() {

    const releases = await gitHubHelper.getReleases();
    console.log(releases);
    const dropdown = document.getElementById('select-0454');
    dropdown.innerHTML = '<option style="display:none" disabled selected value>No version currently selected</option>';
    releases.forEach(release => {
        dropdown.innerHTML += `<option value="${release.name}">${release.name}</option>`
    });

}
async function downloadIt(version) {
    const Path = path.join(getPath('appData'), 'swinc-multiplayer-mod');
    const zipPath = path.join(Path, version.name + ".zip");
    await got(`https://api.redirect-checker.net/?url=${version.download}`).then(async resp => {
        const url = JSON.parse(resp.body).data[0].response.info.redirect_url;
        await pipeline(
            got.stream(url),
            fs.createWriteStream(zipPath)
        );
    });
    return;
}
async function downloadRelease(version, force = false) {
    if (!fs.existsSync(path.join(getPath('appData'), 'swinc-multiplayer-mod'))) {
        fs.mkdirSync(path.join(getPath('appData'), 'swinc-multiplayer-mod'));
    }
    const Path = path.join(getPath('appData'), 'swinc-multiplayer-mod');
    const zipPath = path.join(Path, version.name + ".zip");
    const dirPath = path.join(Path, version.name);
    const installDir = settings.getSync('installDir');
    await downloadIt(version)
    await extract(zipPath, {
        dir: dirPath
    });
    console.log('Extraction complete, installing mod now.');
    await copyFolder(path.join(dirPath, 'manage'), path.join(installDir.toString(), 'Software Inc_Data', 'Managed'));
    await copyFolder(path.join(dirPath, 'mf'), path.join(installDir.toString(), 'DLLMods', 'Multiplayer'));
    console.log('Installed Mod.');
}
async function openURL(url) {
    open(url);
}
async function copyFolder(from, to) {
    await ncp(from, to, function (err) {
        if (err) {
            throw err;
        }
        console.log(`Copied ${from} to ${to}`);
    });
}

function onIndexChanges() {
    const dropdown = document.getElementById('select-0454');
    const version = JSON.parse(localStorage.getItem(dropdown.options[dropdown.selectedIndex].text));
    let stringe = version.name + " - Stable";
    if (version.unstable) stringe = version.name + " - Unstable - Be wary, may contain bugs.";
    document.getElementById('frame').innerHTML = stringe;
    document.getElementById('githubButton').setAttribute('onclick', `openURL("${version.url}")`)
}
async function viewOnGitHub() {
    const dropdown = document.getElementById('select-0454');
    const text = $("#select-0454 :selected").text().toLowerCase()
    if (text.includes('placeholder') || text.includes('no version currently selected')) {
        bootbox.alert("Please select a valid version from the dropdown.");
        return;
    }
    const version = JSON.parse(localStorage.getItem(dropdown.options[dropdown.selectedIndex].text));
}
async function install() {
    const dropdown = document.getElementById('select-0454');
    const text = $("#select-0454 :selected").text().toLowerCase()
    if (text.includes('placeholder') || text.includes('no version currently selected')) {
        return bootbox.alert("Please select a valid version from the dropdown."); 
    }
    const e = await gitHubHelper.getInstallationDir()
    console.log(`Installation Directory:` + e);
    const version = JSON.parse(localStorage.getItem(dropdown.options[dropdown.selectedIndex].text));
    startLoading();
    await downloadRelease(version);
    stopLoading();
    bootbox.alert("Successfully installed the multiplayer mod.");
}
$body = $("body");

function startLoading() {
    $body.addClass("loading");
}

function stopLoading() {
    $body.removeClass("loading");
}

load();
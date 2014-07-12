function removedStatusRefreshPage(checkBox) {
    var status = checkBox.checked;
    var url = window.location.href;
    url = url.replace("?showRemoved=true", "").replace("?showRemoved=false", "");
    window.location.href = url + "?showRemoved=" + status;
}
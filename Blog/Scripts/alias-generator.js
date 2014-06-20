function generateAlias()
{
    var title = document.getElementById("tb-title").value;
    var aliasTB = document.getElementById("tb-alias");

    title = title.split(' ').join('-').toLowerCase();

    aliasTB.setAttribute("value", title);
}
let index = 0;

function AddTag() {
    var tagEntry = document.getElementById("TagEntry");
    let newOption = new Option(tagEntry.value, tagEntry.value);
    document.getElementById("TagList").options[index++] = newOption;
    tagEntry.value = "";
    return true;
}

function DeleteTag() {
    let tagCount = 1;
    while (tagCount > 0) {
        let selectedIndex = document.getElementById("TagList").selectedIndex;
        if (selectedIndex >= 0) {
            document.getElementById("TagList").options[selectedIndex] = null;
            --tagCount;
        }
        else
            tagCount = 0;
        index--;
    }
}
$("form").on("submit", () => {
    $("#TagList option").prop("selected", "selected");
})
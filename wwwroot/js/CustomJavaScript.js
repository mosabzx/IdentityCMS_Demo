function ConfirmDelete(UniqId, IsDeleteClicked) {
    var deleteSpan = 'deleteSpan_' + UniqId;
    var confirmDeleteSpan = 'confirmDeleteSpan_' + UniqId;

    if (IsDeleteClicked) {
        $('#' + deleteSpan).hide();
        $('#' + confirmDeleteSpan).show();
    } else {
        $('#' + deleteSpan).show();
        $('#' + confirmDeleteSpan).hide();
    }
}
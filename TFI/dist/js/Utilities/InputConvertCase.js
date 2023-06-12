export function convertToLowerCase(selector){
    if (!$(selector).val()) return;
    let input = $(selector).val().toLowerCase();
    $(selector).val(input);
}

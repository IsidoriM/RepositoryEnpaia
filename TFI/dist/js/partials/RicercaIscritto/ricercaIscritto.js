import {Form} from "./Selector.js";

$(document).ready(() => {
   subscribeToSubmit();
    
   let mutationObserver = new MutationObserver(() => {
       subscribeToSubmit();
   })
    mutationObserver.observe($(Form.partial).get(0), {
        childList: true,
        subtree: true
    })
    function subscribeToSubmit(){
        $(Form.submitButton).click((e) => {
            e.preventDefault();

            $.ajax({
                url: '/Infortuni/RicercaIscritto',
                type: 'POST',
                data: $(Form.form).serialize(),
                success: function (result) {
                    $(Form.partial).html(result);
                },
                error: function (xhr, status, error) {
                    console.error(error);
                }
            });
        });
    }
});
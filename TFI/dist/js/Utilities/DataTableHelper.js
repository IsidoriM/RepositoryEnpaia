function GetInitializerOptionsWithUniformedPaging(tableId) {
    return {
        "paging": true,
        "pagingType": "full",
        "lengthMenu": [[5, 10, 25], [5, 10, 25]],
        'pageLength': 10,
        "ordering": false,
        "lengthChange": true,
        "responsive": true,
        "autoWidth": false,
        "info": true,
        "searching": false,
        'dom': '<"top">rt<"row d-flex justify-content-end w-100 align-items-center mb-0 h-75"lpi><"clear">',
        'language': {
            'lengthMenu': '_MENU_',
            'paginate': {
                'previous': '<',
                'next': '>',
                'first': '<<',
                'last': '>>',
            },
            'infoFiltered' : '',
            'infoEmpty' : '0/0',
            'info': '_PAGE_/_PAGES_'
        },
        'initComplete': function () {
            let lenghtMenu = document.getElementsByName(`${tableId}_length`)[0];
            lenghtMenu.className = 'page-link page-number m-3 pagination';

            $('ul.pagination li').removeClass('paginate_button');
            $('ul.pagination li').removeClass('disabled');

            $(`#${tableId}_paginate`).addClass('pt-2');
            $(`#${tableId}_paginate`).addClass('mx-3');

            $(`#${tableId}_info`).addClass('pb-2');
        },
        'drawCallback': function () {
            $('ul.pagination li').removeClass('paginate_button');
            $('ul.pagination li').removeClass('disabled');
        }
    };
}

export function GetInitializerOptionsWithUniformedPagingAndSearchingBox(tableId){
    let config = GetInitializerOptionsWithUniformedPaging(tableId);
    config.searching = true;
    config.dom = '<"top">ft<"row d-flex justify-content-end w-100 align-items-center mb-0 h-75"lpi><"clear">r';
    config.oLanguage = { "sSearch": "Ricerca :", "sZeroRecords": "Nessun risultato trovato" };
    return config;
}
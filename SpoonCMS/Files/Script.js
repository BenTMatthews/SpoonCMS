var _basePath = "[!CustomPathMarker!]";
var containerAddDialog;
var containerAddDialogForm;
var containerEditDialog;
var containerEditDialogForm;
var itemNameDialog;
var itemNameDialogForm;

$(document).ready(function () {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-center",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    //$.modal.defaults = {
    //    closeExisting: true,    // Close existing modals. Set this to false if you need to stack multiple modal instances.
    //    escapeClose: true,      // Allows the user to close the modal by pressing `ESC`
    //    clickClose: false,       // Allows the user to close the modal by clicking the overlay
    //    closeText: 'Close',     // Text content for the close <a> tag.
    //    closeClass: '',         // Add additional class(es) to the close <a> tag.
    //    showClose: true,        // Shows a (X) icon/link in the top-right corner
    //    modalClass: "modal",    // CSS class added to the element being displayed in the modal.
    //    blockerClass: "modal",  // CSS class added to the overlay (blocker).
    //    showSpinner: true,      // Enable/disable the default spinner during AJAX requests.
    //    fadeDuration: null,     // Number of milliseconds the fade transition takes (null means no transition)
    //    fadeDelay: 1.0          // Point during the overlay's fade-in that the modal begins to fade in (.5 = 50%, 1.5 = 150%, etc.)
    //};

    UpdateContainerList();

    $(document).on('click', '.containerItem', function (e) {
        let id = $(this).data("container-id");

        $("#containerList .containerItem").removeClass("selected");
        $(this).addClass("selected");

        LoadContainer(id);
    });  

    $(document).on('click', '#containerListOptions button.addContainer', function (e) {
        containerAddDialog.dialog("open");
    });

    $(document).on('click', '#containerDetails button.saveContainer', function (e) {
        let conId = $(this).data("container-id");
        let container = BuildContainer(conId);
        SaveContainer(conId, container);
    });

    $(document).on('click', '#containerDetailsAttributes .contentDetailsBlock .contentSaveBlock button.saveItem', function (e) {
        let containerId = $(this).data("container-id");
        let contentId = $(this).data("item-id");
        let contentItem = BuildContentItem(contentId);
        SaveContentItem(containerId, contentItem);
    });   

    $(document).on('click', '.itemBlock:not(.addItem)', function (e) {
        let itemId = $(this).data("item-id");
        $(".contentDetailsBlock").hide();
        $(".contentDetailsBlock[data-item-id='" + itemId + "']").show();

        $("#containerItems .itemBlock").removeClass("active");
        $(this).addClass("active");
    });        

    $(document).on('click', '#containerItems .itemBlock.addItem', function (e) {
        let itemEditor = $("#contentDetails-partial").html();
        let itemEditorTemplate = Handlebars.compile(itemEditor);

        let itemBlockHtml = $("#contentBlock-partial").html();
        let itemBlockTemplate = Handlebars.compile(itemBlockHtml);

        let itemObj = {};
        itemObj.Name = "New Item";
        itemObj.Id = Guid();
        itemObj.value = "";

        let htmlEditor = itemEditorTemplate(itemObj);
        let htmlBlock = itemBlockTemplate(itemObj);

        $("#containerDetailsAttributes").append(htmlEditor);
        CKEDITOR.replace(itemObj.Id).config.allowedContent = true;
        //CKEDITOR.instances[itemObj.Id].config.startupMode = 'source'

        $("#containerItems").append(htmlBlock);

        //Do some cleanup since we don't have @root in partials, get container level data in 
        let conId = $(this).data("container-id");
        let conName = $("#containerList .containerItem[data-container-id='" + conId + "']").first().text();
        $(".contentDetailsBlock[data-item-id='" + itemObj.Id + "'] .contentName .containerName").text(conName);
        $(".contentDetailsBlock[data-item-id='" + itemObj.Id + "'] .contentSaveBlock button.saveItem").attr("data-container-id", conId);

        UpdatePriorityCounts();

        $("#containerItems .itemBlock[data-item-id='" + itemObj.Id + "']").trigger('click');
    });

    $(document).on('click', '#containerDetails .contentDetailsBlock .contentName i.editContentName', function (e) {
        let contentId = $(this).data("item-id");
        let currName = $(".contentDetailsBlock[data-item-id='" + contentId + "'] .contentName .containerItemName").first().text();

        $('#dialog-itemName #editItemNameSave').attr("data-content-id", contentId);
        $('#dialog-itemName #formItemName').val(currName);

        $('#dialog-itemName').modal();
        $('#dialog-itemName #formItemName').focus();
    });

    console.log("SpoonCMS admin page ready!");
});

//GENERAL FUNCTIONS
function BuildContainer(conId) {
    let container = {};

    container.Id = conId;
    container.Active = true;
    container.Name = $("#containerDetails .containerName .containerNameBlock").first().text();

    let itemsDictionary = {};

    $("#containerDetailsAttributes .contentDetailsBlock").each(function () {
        let contentId = $(this).data("item-id");

        let contentItem = BuildContentItem(contentId);

        itemsDictionary[contentItem.Name] = contentItem;
    });

    container.Items = itemsDictionary;

    return container;
}

function BuildContentItem(contentId) {
    let contentIem = {};
    let dataBlock = $(".contentDetailsBlock[data-item-id='" + contentId + "']").first();
    contentIem.Id = contentId;
    contentIem.Active = true;
    contentIem.Name = $(dataBlock).find(".contentName .containerItemName").first().text();
    contentIem.Priority = $("#containerItems .itemBlock .contentPriority").first().text();

    let editor = CKEDITOR.instances[contentId];
    editor.updateElement();

    contentIem.Value = editor.getData();

    return contentIem;
}

function UpdatePriorityCounts() {
    $("#containerItemsAccordion .group").each(function (index) {
        $(this).find("h3.itemName .itemPriority").html(index + 1);
    });
}

function Guid() {
    return S4() + S4() + '-' + S4() + '-' + S4() + '-' +
        S4() + '-' + S4() + S4() + S4();
}

function S4() {
    return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
}

//Service Functions
function UpdateContainerList(conId) {
    $.ajax({
        type: "GET",
        url: _basePath + "/GetContainers",
        success: function (result, status) {
            containers = JSON.parse(result);
            if (containers.Success) {
                $("#containerList").html("");
                let containerListHb = $("#container-list-template").html();
                let containerListTemplate = Handlebars.compile(containerListHb);
                Handlebars.registerPartial("contentBlock-partial", $("#contentBlock-partial").html());
                Handlebars.registerPartial("contentDetails-partial", $("#contentDetails-partial").html());

                let html = containerListTemplate(containers.Data);
                $("#containerList").append(html);

                if (conId) {
                    $("#containerList .containerItem[data-container-id='" + conId + "']").click();
                }
            }
            else {
                toastr["error"](containers.Message);
            }
        },
        error: function (jqXHR) {
            toastr["error"](jqXHR);
        }
    });
}

function SaveContainer(conId, container) {
    $.ajax({
        type: "POST",
        url: _basePath + "/SaveContainer?id=" + conId,
        data: JSON.stringify(container),
        success: function (result, status) {
            data = JSON.parse(result);
            if (data.Success) {
                toastr["success"]("Container saved!");
            }
            else {
                toastr["error"](data.Message);
            }
        },
        error: function (jqXHR) {
            toastr["error"](jqXHR);
        }
    });
}

function SaveContentItem(conId, contentItem) {
    $.ajax({
        type: "POST",
        url: _basePath + "/SaveContentItem?id=" + conId,
        data: JSON.stringify(contentItem),
        success: function (result, status) {
            data = JSON.parse(result);
            if (data.Success) {
                toastr["success"](contentItem.Name + " saved!");
            }
            else {
                toastr["error"](data.Message);
            }
        },
        error: function (jqXHR) {
            toastr["error"](jqXHR);
        }
    });
}

function AddContainer() {

    let name = $("#formContainerName").val();

    $.ajax({
        type: "POST",
        url: _basePath + "/CreateContainer?name=" + name,
        data: '',
        success: function (result, status) {
            data = JSON.parse(result);
            if (data.Success) {
                toastr["success"]("Container created!");
                $("#formContainerName").val('');
                UpdateContainerList();
            }
            else {
                toastr["error"](data.Message);
            }
        },
        error: function (jqXHR) {
            toastr["error"](jqXHR);
            $("#formContainerName").val();
        }
    });
}

function LoadContainer(id) {
    $.ajax({
        type: "GET",
        url: _basePath + "/GetContainer?id=" + id,
        success: function (result, status) {
            container = JSON.parse(result);
            if (container.Success) {
                $("#containerDetails").html("");
                let containerDetailsHb = $("#container-details-template").html();
                let containerDetailsTemplate = Handlebars.compile(containerDetailsHb);
                let html = containerDetailsTemplate(container.Data);
                $("#containerDetails").append(html);                

                CKEDITOR.replaceAll('htmlEditBox');

                for (name in CKEDITOR.instances) {
                    CKEDITOR.instances[name].config.allowedContent = true;
                    //CKEDITOR.instances[name].config.startupMode = 'source';
                }

                UpdatePriorityCounts();

                $("#containerItems").children(".itemBlock:not(.addItem)").first().trigger("click");
            }
            else {
                toastr["error"](container.Message);
            }
        },
        error: function (jqXHR) {
            toastr["error"](jqXHR);
        }
    });
}

function DeleteContainer(conName) {
    $.ajax({
        type: "POST",
        url: _basePath + "/DeleteContainer?name=" + conName,
        data: '',
        success: function (result, status) {
            data = JSON.parse(result);
            if (data.Success) {
                toastr["success"]("Container delted!");
                $("#containerDetails").html("");
                UpdateContainerList();
            }
            else {
                toastr["error"](data.Message);
            }
        },
        error: function (jqXHR) {
            toastr["error"](jqXHR);
            $("#formContainerName").val();
        }
    });
}

function UpdateContainerName(conId, conName) {
    $.ajax({
        type: "POST",
        url: _basePath + "/EditContainerName?id=" + conId + "&name=" + conName,
        data: '',
        success: function (result, status) {
            data = JSON.parse(result);
            if (data.Success) {
                toastr["success"]("Container name updated!");
                $("#containerDetailsAttributes #containerName").html(conName);
                UpdateContainerList();
            }
            else {
                toastr["error"](data.Message);
            }
        },
        error: function (jqXHR) {
            toastr["error"](jqXHR);
            $("#formContainerName").val();
        }
    });
}

function UpdateItemName(itemId) {
    let itemNameHeader = $("#containerItemsAccordion").find("h3.itemName[data-item-id='" + itemId + "'] span.itemNameSpan");

    itemNameHeader.html($("#formItemName").val());
    $("#formItemName").value = "";
}

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

    containerAddDialog = $("#dialog-addContainer").dialog({
        autoOpen: false,
        height: 250,
        width: 400,
        modal: true,
        buttons: {
            "Add Container": function () {
                AddContainer();
                containerAddDialog.dialog("close");
            },
            Cancel: function () {
                containerAddDialog.dialog("close");
            }
        },
        close: function () {
            containerAddDialogForm[0].reset();
            containerAddDialogForm.find("#formContainerName").removeClass("ui-state-error");
        }
    });    

    var containerAddDialogForm = containerAddDialog.find("form").on("submit", function (event) {
        event.preventDefault();
        AddContainer();
    });

    containerEditDialog = $("#dialog-nameContainer").dialog({
        autoOpen: false,
        height: 250,
        width: 400,
        modal: true,
        buttons: {
            "Edit Container": function () {
                let conId = $(this).data("container-id");
                let conName = $("#formEditContainerName").val();
                UpdateContainerName(conId, conName);

                containerEditDialog.dialog("close");
            },
            Cancel: function () {
                containerEditDialog.dialog("close");
            }
        },
        close: function () {
            containerEditDialogForm[0].reset();
            containerAddDialogForm.find("#formContainerName").removeClass("ui-state-error");
        }
    });

    var containerEditDialogForm = containerEditDialog.find("form").on("submit", function (event) {
        event.preventDefault();
        let conId = $(this).data("container-id");
        let container = BuildContainer(conId);
        SaveContainer(conId, container);
    });

    itemNameDialog = $("#dialog-itemName").dialog({
        autoOpen: false,
        height: 250,
        width: 350,
        modal: true,
        buttons: {
            "Edit Name": function () {
                UpdateItemName($(this).data('itemId'));
                itemNameDialog.dialog("close");
            },
            Cancel: function () {
                itemNameDialog.dialog("close");
                $("#formItemName").value = "";
            }
        },
        close: function () {
            itemNameDialogForm[0].reset();
            itemNameDialogForm.find("#formItemName").removeClass("ui-state-error");
        }
    });

    var itemNameDialogForm = itemNameDialog.find("form").on("submit", function (event) {
        event.preventDefault();
        UpdateItemName($(this).attr("item-id"));
    });

    UpdateContainerList();

    $(document).on('click', '#containerListOptions button.addContainer', function (e) {
        containerAddDialog.dialog("open");
    });

    $(document).on('click', '#containerButtons button.saveContainer', function (e) {
        let conId = $(this).data("container-id");
        let container = BuildContainer(conId);
        SaveContainer(conId, container);
    });

    $(document).on('click', '#containerButtons button.editContainerName', function (e) {
        let conId = $(this).data("container-id");
        containerEditDialogForm.find("input#container-name-submit").data("container-id", conId);

        $("#formEditContainerName").val($("#containerDetailsAttributes #containerName").html());
        containerEditDialog.data('containerId', conId).dialog("open");
    });

    $(document).on('click', '#containerButtons button.deleteContainer', function (e) {
        //let conId = $(this).data("container-id");
        let conName = $("#containerDetailsAttributes #containerName").html();
        if (confirm("Are you sure you want to delete this " + conName + " forever?")) {
            DeleteContainer(conName);
        }
    });

    $(document).on('click', '#containerActions button.addItem', function (e) {
        let itemHtml = $("#item-partial-template").html();
        let itemTemplate = Handlebars.compile(itemHtml);
        let itemObj = {};
        itemObj.Name = "New Item";
        itemObj.Id = Guid();
        itemObj.value = "";

        let html = itemTemplate(itemObj);

        $("#containerItemsAccordion").append(html);
        $("#containerItemsAccordion").accordion("refresh");
        CKEDITOR.replace(itemObj.Id).config.allowedContent = true;
        UpdatePriorityCounts();
    });

    $(document).on('click', '.itemOptions button.editItem', function (e) {
        let itemId = $(this).data("item-id");
        $("#formItemName").val($("#containerItemsAccordion").find("h3.itemName[data-item-id='" + itemId + "'] span.itemNameSpan").html());
        itemNameDialogForm.find("input#item-name-edit-submit").data("item-id", itemId);
        itemNameDialog.data('itemId', itemId).dialog("open");
    });

    $(document).on('click', '.itemOptions button.deleteItem', function (e) {
        if (confirm("Are you sure you want to delete this item forever?")) {
            let itemId = $(this).data('item-id');
            $("#containerItemsAccordion div.group[data-item-id='" + itemId + "']").remove();
        }
    });    

    $(document).on('click', '.containerItem', function (e) {
        let id = $(this).data("container-id");
        LoadContainer(id);
    });    

    console.log("SpoonCMS admin page ready!");
});

//Container Fucntions
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
                Handlebars.registerPartial("item-partial", $("#item-partial-template").html());

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

                $("#containerItemsAccordion")
                    .accordion({
                        header: "> div > h3",
                        heightStyle: "content",
                        collapsible: true
                    })
                    .sortable({
                        axis: "y",
                        handle: "h3",
                        start: function (event, ui) {
                            let editorName = ui.item.find("textarea.htmlEditBox").attr("id");
                            let editor = CKEDITOR.instances[editorName];
                            editor.updateElement();
                            editor.destroy();
                            CKEDITOR.remove(editor);
                        },
                        stop: function (event, ui) {
                            // IE doesn't register the blur when sorting
                            // so trigger focusout handlers to remove .ui-state-focus
                            ui.item.children("h3").triggerHandler("focusout");

                            // Refresh accordion to handle new order
                            let editorName = ui.item.find("textarea.htmlEditBox").attr("id");
                            $(this).accordion("refresh");
                            CKEDITOR.replace(editorName).config.allowedContent = true;
                            UpdatePriorityCounts();
                        }
                    });

                CKEDITOR.replaceAll('htmlEditBox');

                for (name in CKEDITOR.instances) {
                    CKEDITOR.instances[name].config.allowedContent = true;
                }

                UpdatePriorityCounts();
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

//GENERAL FUNCTIONS
function BuildContainer(conId) {
    let container = {};

    container.Id = conId;
    container.Active = true;
    container.Name = $("#containerDetailsAttributes #containerName")[0].innerText;

    let itemsDictionary = {};

    $("#containerItemsAccordion .group").each(function () {
        let item = {};

        item.Name = $(this).find("h3.itemName .itemNameSpan").html();
        item.Priority = $(this).find("h3.itemName .itemPriority").html();
        item.Active = true;
        item.Id = $(this).find("textarea.htmlEditBox").attr("id");

        let editor = CKEDITOR.instances[$(this).find("textarea.htmlEditBox").attr("id")];
        editor.updateElement();

        item.Value = editor.getData();

        itemsDictionary[item.Name] = item;
    });

    container.Items = itemsDictionary;

    return container;
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
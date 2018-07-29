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

    $(document).on('click', '#containerList .containerItem', function (e) {
        let id = $(this).attr("data-container-id");

        $("#containerList .containerItem").removeClass("selected");
        $(this).addClass("selected");

        LoadContainer(id);
    });

    $(document).on('click', '#containerList .containerItem i.deleteContainer', function (e) {
        if (confirm("Are you sure you want to delete this container and all items?") == true) {
            let name = $(this).attr("data-container-name");
            DeleteContainer(name);
        }
        else {

        }
    });

    $(document).on('click', '#containerListOptions button.addContainer', function (e) {
        containerAddDialog.dialog("open");
    });

    $(document).on('click', '#containerDetails button.saveContainer', function (e) {
        let conId = $(this).attr("data-container-id");
        let container = BuildContainer(conId);

        if (container != false) {
            SaveContainer(conId, container);
        }
    });

    $(document).on('click', '#containerDetailsAttributes .contentDetailsBlock .contentSaveBlock i.saveItem', function (e) {
        let containerId = $(this).attr("data-container-id");
        let contentId = $(this).attr("data-item-id");
        let contentItem = BuildContentItem(contentId);
        SaveContentItem(containerId, contentItem);
    });

    $(document).on('click', '#containerItems .itemBlock', function (e) {
        let itemId = $(this).attr("data-item-id");
        $(".contentDetailsBlock").hide();
        $(".contentDetailsBlock[data-item-id='" + itemId + "']").show();

        $("#containerItems .itemBlock").removeClass("active");
        $(this).addClass("active");
    });

    $(document).on('click', '#containerItems .addItem', function (e) {
        AddItemToContainer();
    });

    $(document).on('click', '#containerDetailsAttributes .contentDetailsBlock .contentName .deleteItemBlock i.deleteItem', function (e) {
        if (confirm("Are you sure you want to delete this item from the container?") == true) {
            let contentId = $(this).attr("data-item-id");
            RemoveItemFromContainer(contentId);
        }
        else {

        }
    });


    //Dialog Handlers
    $(document).on('click', '#containerDetails .contentDetailsBlock .contentName i.editContentName', function (e) {
        e.preventDefault();

        let contentId = $(this).attr("data-item-id");
        let currName = $(".contentDetailsBlock[data-item-id='" + contentId + "'] .contentName .containerItemName").first().text();

        $('#dialog-itemName form').attr("data-item-id", contentId);
        $('#dialog-itemName #formItemName').val(currName);

        $('#dialog-itemName').modal();
        $('#dialog-itemName #formItemName').select();
    });

    $(document).on('click', '#dialog-itemName #editItemNameSave', function (e) {
        $('#dialog-itemName form').submit();
    });

    $('#dialog-itemName form').submit(function (e) {
        e.preventDefault();
        let contentId = $(this).attr("data-item-id");
        UpdateItemName(contentId);
    });


    $(document).on('click', '#containerDetails #containerName i.editContainerName', function (e) {
        e.preventDefault();

        let conId = $(this).attr("data-container-id");
        let currName = $("#containerDetails #containerName .containerNameBlock").first().text();

        $('#dialog-nameContainer form').attr("data-container-id", conId);
        $('#dialog-nameContainer #formEditContainerName').val(currName);

        $('#dialog-nameContainer').modal();
        $('#dialog-nameContainer #formEditContainerName').select();
    });

    $(document).on('click', '#dialog-nameContainer #editContainerNameSave', function (e) {
        $('#dialog-nameContainer form').submit();
    });

    $('#dialog-nameContainer form').submit(function (e) {
        e.preventDefault();
        let conId = $(this).attr("data-container-id");
        let newName = $('#dialog-nameContainer #formEditContainerName').val();
        UpdateContainerName(conId, newName);
    });


    $(document).on('click', '#containerList #containerListOptions #addContainer', function (e) {
        e.preventDefault();

        $('#dialog-addContainer').modal();
        $('#dialog-addContainer #formContainerName').val("Container name");
        $('#dialog-addContainer #formContainerName').select();
    });

    $(document).on('click', '#dialog-addContainer #addContainerAdd', function (e) {
        $('#dialog-addContainer form').submit();
    });

    $('#dialog-addContainer form').submit(function (e) {
        e.preventDefault();
        let newName = $('#dialog-addContainer #formContainerName').val();
        AddContainer(newName);
    });


    //Load Up functions
    UpdateContainerList();

    console.log("SpoonCMS admin page ready!");
});

//GENERAL FUNCTIONS

function AddItemToContainer() {
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
    CKEDITOR.instances[itemObj.Id].config.autoParagraph = false;
    //CKEDITOR.instances[itemObj.Id].config.startupMode = 'source'

    $("#containerItems").append(htmlBlock);

    //Do some cleanup since we don't have @root in partials, get container level data in 
    let conId = $(this).attr("data-container-id");
    let conName = $("#containerList .containerItem[data-container-id='" + conId + "']").first().text();
    $(".contentDetailsBlock[data-item-id='" + itemObj.Id + "'] .contentName .containerName").text(conName);
    $(".contentDetailsBlock[data-item-id='" + itemObj.Id + "'] .contentSaveBlock button.saveItem").attr("data-container-id", conId);

    UpdatePriorityCounts();

    $("#containerItems .itemBlock[data-item-id='" + itemObj.Id + "']").trigger('click');
}

function RemoveItemFromContainer(itemId) {
    $("#containerDetailsAttributes .contentDetailsBlock[data-item-id='" + itemId + "']").remove();
    $("#containerItems .itemBlock[data-item-id='" + itemId + "']").remove();

    $("#containerItems").children(".itemBlock").first().trigger("click");
}

function UpdateItemName(itemId) {
    let newName = $('#dialog-itemName #formItemName').val();

    $(".contentDetailsBlock[data-item-id='" + itemId + "'] .contentName .containerItemName").first().text(newName);
    $("#containerItems .itemBlock[data-item-id='" + itemId + "'] .itemContentBlock").first().text(newName)

    $.modal.close();

    toastr["success"]('Content name changed to "' + newName + '"');
}

function BuildContainer(conId) {
    let container = {};

    container.Id = conId;
    container.Active = true;
    container.Name = $("#containerDetails #containerName .containerNameBlock").first().text();

    let itemsDictionary = {};

    let validItems = true;

    $("#containerItems .itemBlock").each(function () {
        let contentId = $(this).attr("data-item-id");
        let contentItem = BuildContentItem(contentId);

        if (!(contentItem.Name in itemsDictionary)) {
            itemsDictionary[contentItem.Name] = contentItem;
        } else {
            toastr["error"]("Content Items must have unique names");
            validItems = false;
            return false; //breaks out of each loop, not function
        }
    });

    if (validItems) {
        container.Items = itemsDictionary;
        return container;
    } else {
        return false;
    }

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
    $("#containerDetails #containerItems .itemBlock").each(function (index) {
        $(this).find(".contentPriority").text(index + 1);
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

function UpdateContainerList(conName) {
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

                if (conName) {
                    $("#containerList .containerItem").removeClass("selected");
                    $("#containerList .containerItem[data-container-name='" + conName + "']").addClass("selected");
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

function AddContainer(conName) {
    $.ajax({
        type: "POST",
        url: _basePath + "/CreateContainer?name=" + conName,
        data: '',
        success: function (result, status) {
            data = JSON.parse(result);
            if (data.Success) {
                toastr["success"]("Container created!");
                $("#formContainerName").val('');
                UpdateContainerList();
                $.modal.close();
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
                    CKEDITOR.instances[name].config.autoParagraph = false;
                    //CKEDITOR.instances[name].config.startupMode = 'source';
                }

                UpdatePriorityCounts();

                var list = document.getElementById("containerItems");
                Sortable.create(list, {
                    draggable: ".itemBlock",
                    onUpdate: function (evt/**Event*/) {
                        UpdatePriorityCounts();
                    }
                });

                $("#containerItems").children(".itemBlock").first().trigger("click");
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
                $.modal.close();
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
                $("#containerDetails #containerName .containerNameBlock").first().text(conName);
                UpdateContainerList(conName);
                $.modal.close();
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


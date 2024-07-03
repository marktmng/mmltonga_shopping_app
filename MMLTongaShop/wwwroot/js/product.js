/*This is the javascript for the MESSAGE to show when DELETING products*/

function Delete(url){
    swal({
        title: "Click OK if you want to delete the item",
        text: "If click OK it will be deleted permanently",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);

                        setTimeout(reloadPage,3000); /* set timeout */
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    });
}

function reloadPage() {
    window.location.reload();
}
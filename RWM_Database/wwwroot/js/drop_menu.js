function myFunction(name) {
    document.getElementById(name).classList.toggle("show");
}

function filterFunction(name, myInput) {
    var input, filter, ul, li, a, i;
    input = document.getElementById(myInput);
    filter = input.value.toUpperCase();
    div = document.getElementById(name);
    a = div.getElementsByTagName("a");
    for (i = 0; i < a.length; i++) {
        txtValue = a[i].textContent || a[i].innerText;
        if (txtValue.toUpperCase().indexOf(filter) > -1) {
            a[i].style.display = "";
        } else {
            a[i].style.display = "none";
        }
    }
}
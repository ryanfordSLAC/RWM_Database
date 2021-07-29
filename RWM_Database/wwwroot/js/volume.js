

    function volume() {
        var height = document.getElementById('Height').value;
        var length = document.getElementById('Length').value;
        var width = document.getElementById('Width').value;
        var result = parseFloat(height) * parseFloat(length) * parseFloat(width);
        if (!isNaN(result)) {
            document.getElementById('Volume').innerHTML = result.toFixed(2);
        }
        this.calcAddedVolume();
    }

    function calcAddedVolume() {
        var myselect = document.getElementById("ContainerNumber");
        var container_id = myselect.options[myselect.selectedIndex].value;
        var containerVolume = document.getElementById("hidden_used_" + container_id).value;
        var containerTotal = document.getElementById("hidden_total_" + container_id).value;

        var currentVolume = document.getElementById('Volume').innerHTML;

        var totalVolume = parseFloat(containerVolume) + parseFloat(currentVolume);

        var percentFilled = totalVolume / parseFloat(containerTotal) * 100;

        console.log(percentFilled);
        console.log(parseFloat(containerTotal));

        if (!isNaN(containerVolume)) {
            document.getElementById('PreviousVolume').innerHTML = parseFloat(containerVolume).toFixed(2);
        }

        if (!isNaN(totalVolume)) {
            document.getElementById('PostVolume').innerHTML = totalVolume.toFixed(2);
        }

        if (!isNaN(percentFilled)) {
            document.getElementById('PercentFilled').innerHTML = percentFilled.toFixed(2);
        }
    }
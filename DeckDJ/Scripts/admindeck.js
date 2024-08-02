function createEventListeners(DeckId, UserId) {
    $(document).on("click", ".search-results-card", function () {
        searchResultCardButtonLeftClick(this); //add card to main deck
    });

    $('#search').on('keyup', function (e) {
        if (e.key === 'Enter') {
            getSearchResults(this); //search when enter is pressed
        }
    });
}

//Searches based on text in the search textbox
function getSearchResults(element) {
    if (element.value !== "") {
        let URL = APIURL + "CardData/NameSearch/" + element.value;
        var xhr = new XMLHttpRequest();

        xhr.open("GET", URL, true);

        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                let response = (JSON.parse(xhr.responseText));
                if (response.length !== 0) {
                    let searchResults = (JSON.parse(xhr.responseText));
                    if (searchResults.length > 0) {
                        $("#search-results").empty();
                        for (key in searchResults) {
                            let card = searchResults[key];
                            addSearchResult(card.id, card.Name);
                        }
                    }
                }
            }
        }
        xhr.send();
    }
}


//Creates DOM elements for view Search Results
function addSearchResult(id, name) {
    let cardContainer = document.createElement("div");
    let cardName = document.createElement("p");
    cardName.innerHTML = name;
    let addButton = document.createElement("button");
    addButton.innerHTML = "Add";
    addButton.value = id;
    addButton.addEventListener('click', function () {

    })
    const container = document.getElementById("search-results");
    container.appendChild(cardContainer);
}
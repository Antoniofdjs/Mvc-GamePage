$(document).ready(function () {
    ReviewColors();
});

function ReviewColors() {
    // Map for colors
    var ratingColors = [
        { value: 90, color: '#22c55e', outlineColor: '#22c55e' }, // green-500 for > 89
        { value: 70, color: '#a3e635', outlineColor: '#a3e635' }, // lime-400 for > 79
        { value: 60, color: '#f97316', outlineColor: '#f97316' }, // orange-500 for > 60
        { value: 0, color: '#ef4444', outlineColor: '#ef4444' }   // red-500 for <= 60
    ];

    console.log("gameJS activated");
    var metaCriticRating = $('#metacritic-rating');
    var metaCriticTextBox = $('#metacritic-text');

    var steamRating = $('#steam-rating');
    var steamTextBox = $('#steam-text');

    var igdbRating = $('#igdb-rating');
    var igdbTextBox = $('#igdb-text');

    // 1) Metacitic rating color change
    if (metaCriticRating.length > 0) {
        var metaCriticRatingValue = parseInt(metaCriticRating.text(), 10);
        console.log("MetaCritic Rating Value: " + metaCriticRatingValue);

        var metaCriticColor = ratingColors.find(r => metaCriticRatingValue >= r.value).color;
        metaCriticRating.css('color', metaCriticColor);
        metaCriticRating.css('outline', '3px solid ' + metaCriticColor);
        metaCriticTextBox.css('color', metaCriticColor);
        metaCriticTextBox.css('box-shadow', '0 2px 2px ' + metaCriticColor);

    } else {
        console.log("MetaCritic Rating element does not exist");
    }

    // 2) Steam rating color change
    if (steamRating.length > 0) {
        var steamRatingValue = parseInt(steamRating.text(), 10);
        console.log("Steam Rating Value: " + steamRatingValue);

        var steamColor = ratingColors.find(r => steamRatingValue >= r.value).color;
        steamRating.css('color', steamColor);
        steamRating.css('outline', '3px solid ' + steamColor);
        steamTextBox.css('color', steamColor);
        steamTextBox.css('box-shadow', '0 2px 2px ' + steamColor);
    } else {
        console.log("Steam Rating element does not exist");
    }

    // 3) Igdb rating color change
    if (igdbRating.length > 0) {
        var igdbRatingValue = parseInt(igdbRating.text(), 10);
        console.log("Igdb Rating Value: " + igdbRatingValue);

        var igdbColor = ratingColors.find(r => igdbRatingValue >= r.value).color;
        igdbRating.css('color', igdbColor);
        igdbRating.css('outline', '3px solid ' + igdbColor);
        igdbTextBox.css('color', igdbColor);
        igdbTextBox.css('box-shadow', '0 2px 2px ' + igdbColor);
    } else {
        console.log("Igdb Rating element does not exist");
    }
}
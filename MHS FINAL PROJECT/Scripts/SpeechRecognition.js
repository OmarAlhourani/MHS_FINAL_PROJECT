    const btn_talk = document.querySelector('.talk');
    const Spech = window.SpeechRecognition || window.webkitSpeechRecognition;
    const recongnation = new Spech();
    recongnation.lang = 'en-US';





recongnation.onresult = function (event) {
        const text = event.resultIndex;
        const script = event.results[text][0].transcript;
        if ((window.location.href).includes("Normal_search")) {
            window.location = "Normal_search?model.name=" + script;
        }
        else if ((window.location.href).includes("Drug_Interactions")) {
            window.location = "Drug_Interactions?model.name=" + script;
        }
        else if ((window.location.href).includes("advanced_search"))
        {
            var description = document.querySelector('.description_b').checked;
            var drug_dosage = document.querySelector('.drug_dosage_b').checked;
            var warnings = document.querySelector('.warnings_b').checked;
            var side_effects = document.querySelector('.sideEffects_b').checked;
            var trade_names = document.querySelector('.trade_names_b').checked;
            window.location = "advanced_search?model.search_name=" + script + "&model.description=" + description + "&model.drug_dosage=" + drug_dosage +
                "&model.warnings=" + warnings + "&model.side_effects=" + side_effects + "&model.trade_names=" + trade_names;

        }
        else
        {
            window.location = "home/Normal_search?model.name=" + script;
        }
        
        console.log(event.results);
};


recongnation.onend = function (event) {
    var get_mic = document.querySelector('.mic_add');
    get_mic.classList.remove("AnimateMic");
    console.log('Speech not nomatch');
}


//recongnation.nomatch = function () {
//    var get_mic = document.querySelector('.mic_add');
//    get_mic.classList.remove("AnimateMic");
//}

btn_talk.addEventListener('click', () => {
    var get_mic = document.querySelector('.mic_add');
    get_mic.classList.add("AnimateMic");
    recongnation.start();
});
var params = {
    container: document.getElementById('js-hero-animation'),
    renderer: 'svg',
    loop: 2,
    autoplay: true,
    path: 'assets/data.json'
};

var anim;

anim = lottie.loadAnimation(params);
params.container.onmouseover = playit;
anim.onComplete = function () {
    anim.finished = true;
}


function playit() {
    // console.log("playing");
    if (anim.finished) {
        anim.finished = false;
        anim.goToAndPlay(1, true);
    }
}
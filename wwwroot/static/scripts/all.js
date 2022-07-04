(function () {
  'use strict';

  init();

  // init function to bulk call all functions
  function init () {
    heroAnimation();
    loadMoreHandler();
    addH5YRstyle();
  }

  //
  // loadMoreHandler
  //
  // Handler for the homepage Twitter feed 'load more' button
  //
  function loadMoreHandler () {
    $('.load-more').click(function (e) {
      e.preventDefault();

      $(this).find('span').text('');
      $(this).addClass('loading');
      $(this).find('.spinner').show();

      $.get('/loadmoretweets/index', function (data) {
        $('.tweet__grid').append(data);
        $('.load-more').removeClass('loading');
        $('.load-more').find('.spinner').hide();
        $('.load-more').find('span').text('Load More');
        $('time.timeago').timeago();
        addH5YRstyle();
      });
    })
  }

  //
  // addH5YRstyle
  //
  // Check all tweets for '#h5yr' and wrap a <span> around it for styling.
  // If tweet already contains styled span, ignore.
  //
  function addH5YRstyle () {
    $(".tweet__content a:contains('#h5yr')").html(function (_, html) {
      if (!$(this).children().hasClass('h5yr')) {
        return html.replace(/(#h5yr)/g, '<span class="h5yr">#H5YR</span>');
      }
    });

    $(".tweet__content a:contains('#H5YR')").html(function (_, html) {
      if (!$(this).children().hasClass('h5yr')) {
        return html.replace(/(#H5YR)/g, '<span class="h5yr">#H5YR</span>');
      }
    });
  }

  //
  // heroAnimation
  //
  // Handler for the homepage hero high5 animation
  //
  function heroAnimation () {
    // Define hero animation element
    var $animationContainer = document.getElementById('js-hero-animation');
    var lottie = window.lottie;

    // Set up animation parameters
    var params = {
      container: $animationContainer,
      renderer: 'svg',
      loop: 1,
      autoplay: true,
      path: 'assets/data.json'
    };

    // Load animation parameters into lottie
    var anim = lottie.loadAnimation(params);

    // On mouseover, play animation
    $animationContainer.addEventListener('mouseover', function () {
      playit()
    });

    // On complete event, set anim.finished to true
    anim.addEventListener('complete', function () {
      anim.finished = true
    });

    // If animation isn't running, start animating
    function playit () {
      if (anim.finished) {
        anim.finished = false;
        anim.goToAndPlay(1, true);
      }
    }
  }
})($);

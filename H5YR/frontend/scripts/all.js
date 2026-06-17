import '@justinribeiro/share-to-mastodon';
import TimeAgo from 'javascript-time-ago';
import en from 'javascript-time-ago/locale/en';
import { initCommunityStats } from './community-stats';

// Initialize community stats dashboard
initCommunityStats();

  //
  // heroAnimation
  //
  // Handler for the homepage hero high5 animation
  //

  // Define hero animation element
  var $animationContainer = document.getElementById('js-hero-animation');

  if ($animationContainer) {
    var lottie = window.lottie;

    // Set up animation parameters
    var params = {
      container: $animationContainer,
      renderer: 'svg',
      loop: 1,
      autoplay: true,
      path: '/h5yrAnimation/data.json'
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
    function playit() {
      if (anim.finished) {
        anim.finished = false;
        anim.goToAndPlay(1, true);
      }
    }
  }


  //
  // loadMoreHandler
  //
  // Handler for the homepage Mastodon feed 'load more' button
  //

  document.querySelectorAll('.load-more').forEach(function (el) {
    var loadMoreButton = el;
    el.addEventListener('click', function (e) {
      e.preventDefault();

      el.querySelector('span').textContent = '';
      el.classList.add('loading');
      el.querySelector('.spinner').style.display = 'block';

      var startId = loadMoreButton.getAttribute('data-start-id');
      loadMoreButton.setAttribute('data-start-id', '');

      // Get the oldest date from the last feed item for widget pagination
      var oldestDate = loadMoreButton.getAttribute('data-oldest-date') || '';

      fetch('api/loadmoreposts/?startId=' + startId + '&oldestDate=' + encodeURIComponent(oldestDate)).then(function (response) {
        return response.text();
      }).then(function (data) {
        document.querySelector('.tweet__grid').insertAdjacentHTML('beforeend', data);

        // Update the oldest date from the newly loaded items
        var allTimestamps = document.querySelectorAll('.tweet__grid .timeago');
        if (allTimestamps.length > 0) {
          var lastTimestamp = allTimestamps[allTimestamps.length - 1].getAttribute('datetime');
          loadMoreButton.setAttribute('data-oldest-date', lastTimestamp);
        }

        loadMoreButton.classList.remove('loading');
        loadMoreButton.querySelector('.spinner').style.display = 'none';
        loadMoreButton.querySelector('span').textContent = 'Load More';
        applyTimeAgo();
      });
    });
  });

applyTimeAgo();

  //
  // TimeAgo
  //
  // Replaces datetime strings with relative time e.g. '2 hours ago'
  //

  // TODO: if markup isn't being cached, humanizer would do a better job here
// (if it is, a cloudflare worker could do it better too)

function applyTimeAgo() {
    TimeAgo.addDefaultLocale(en);

    var timeAgo = new TimeAgo('en');
    var timestamps = document.querySelectorAll('.timeago');

    timestamps.forEach(function (timestamp) {
        var date = new Date(timestamp.getAttribute('datetime'));
        timestamp.textContent = timeAgo.format(date);
    });
}

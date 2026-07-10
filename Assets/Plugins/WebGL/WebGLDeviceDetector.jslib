mergeInto(LibraryManager.library, {

  IsMobileBrowser: function () {
    var ua = navigator.userAgent || navigator.vendor || window.opera || "";

    // Match common mobile/tablet user-agent signatures
    var mobileRegex = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini|Mobile|Tablet/i;
    var isUAMobile = mobileRegex.test(ua);

    // iPadOS 13+ reports itself as "Macintosh" in the UA string,
    // so detect it via touch support instead
    var isIPadOS = (navigator.platform === "MacIntel" && navigator.maxTouchPoints > 1);

    // Generic touch capability check (covers most Android tablets/phones)
    var hasTouch = ('ontouchstart' in window) ||
                    (navigator.maxTouchPoints > 0) ||
                    (navigator.msMaxTouchPoints > 0);

    var isMobile = isUAMobile || isIPadOS || (hasTouch && window.innerWidth <= 1024);

    return isMobile ? 1 : 0;
  }

});

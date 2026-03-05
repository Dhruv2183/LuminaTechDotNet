/* =========================================
   LUMINATECH — Hero Scroll Animation
   DIRECT scroll-driven: no lerp delay,
   text changes instantly based on scroll
   position for responsive, professional UX
   ========================================= */

(function () {
    const video = document.getElementById('scroll-video');
    if (video) {
        video.muted = true;
        video.loop = true;
        video.playsInline = true;
        video.play().catch(() => {
            document.addEventListener('click', () => video.play(), { once: true });
            document.addEventListener('scroll', () => video.play(), { once: true });
        });
    }

    const overlay1 = document.getElementById('heroOverlay');
    const overlay2 = document.getElementById('heroOverlay2');
    const overlay3 = document.getElementById('heroOverlay3');
    const indicator = document.getElementById('scrollIndicator');

    // Easing function — makes transitions feel smooth and natural
    function easeInOutCubic(t) {
        return t < 0.5 ? 4 * t * t * t : 1 - Math.pow(-2 * t + 2, 3) / 2;
    }

    function handleScroll() {
        const hero = document.getElementById('hero');
        if (!hero) return;
        const rect = hero.getBoundingClientRect();
        const scrolled = -rect.top;
        const total = hero.offsetHeight - window.innerHeight;
        const p = Math.min(Math.max(scrolled / total, 0), 1);

        // Phase 1: 0 → 0.40 — overlay 1 visible, fades out 0.30 → 0.40
        // Phase 2: 0.30 → 0.72 — overlay 2 crossfades in 0.30→0.42, out 0.60→0.72
        // Phase 3: 0.60 → 1.0 — overlay 3 fades in 0.60→0.75, stays
        // (Overlapping ranges create smooth crossfade, no gap where nothing is visible)

        let o1 = 0, o2 = 0, o3 = 0;
        let y1 = 0, y2 = 0, y3 = 0;

        // Overlay 1
        if (p <= 0.30) {
            o1 = 1;
            y1 = p / 0.30 * -8;
        } else if (p <= 0.42) {
            const t = (p - 0.30) / 0.12;
            o1 = 1 - easeInOutCubic(t);
            y1 = -8 - t * 20;
        } else {
            o1 = 0;
            y1 = -28;
        }

        // Overlay 2
        if (p >= 0.30 && p <= 0.42) {
            const t = (p - 0.30) / 0.12;
            o2 = easeInOutCubic(t);
            y2 = (1 - easeInOutCubic(t)) * 30;
        } else if (p > 0.42 && p <= 0.60) {
            o2 = 1;
            y2 = 0;
        } else if (p > 0.60 && p <= 0.72) {
            const t = (p - 0.60) / 0.12;
            o2 = 1 - easeInOutCubic(t);
            y2 = -t * 20;
        } else if (p > 0.72) {
            o2 = 0;
            y2 = -20;
        } else {
            o2 = 0;
            y2 = 30;
        }

        // Overlay 3
        if (p >= 0.60 && p <= 0.75) {
            const t = (p - 0.60) / 0.15;
            o3 = easeInOutCubic(t);
            y3 = (1 - easeInOutCubic(t)) * 30;
        } else if (p > 0.75) {
            o3 = 1;
            y3 = 0;
        } else {
            o3 = 0;
            y3 = 30;
        }

        // Apply directly — no delay, pure scroll-driven
        if (overlay1) {
            overlay1.style.opacity = o1;
            overlay1.style.transform = `translateY(${y1}px)`;
        }
        if (overlay2) {
            overlay2.style.opacity = o2;
            overlay2.style.transform = `translateY(${y2}px)`;
        }
        if (overlay3) {
            overlay3.style.opacity = o3;
            overlay3.style.transform = `translateY(${y3}px)`;
        }

        if (indicator) indicator.style.opacity = p > 0.05 ? 0 : 1;
    }

    window.addEventListener('scroll', handleScroll, { passive: true });
    handleScroll();
})();

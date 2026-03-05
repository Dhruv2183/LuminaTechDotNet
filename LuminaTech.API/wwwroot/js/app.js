/* =========================================
   LUMINATECH — Main JavaScript
   GSAP Animations + API Integration
   ========================================= */

// ========== UTILITIES ==========
const API = {
    get: async (url) => {
        const res = await fetch(url);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return res.json();
    },
    post: async (url, data) => {
        const res = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        return res.json();
    },
    put: async (url, data) => {
        const res = await fetch(url, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        return res.json();
    },
    delete: async (url) => {
        const res = await fetch(url, { method: 'DELETE' });
        return res;
    }
};

// ========== AUTH HELPERS ==========
async function checkAuth() {
    try {
        const data = await API.get('/api/auth/status');
        return data;
    } catch {
        return { authenticated: false };
    }
}

async function logout() {
    try {
        await fetch('/api/auth/logout', { method: 'POST' });
    } catch { }
    window.location.href = '/';
}

async function updateNavAuth() {
    const authBtn = document.getElementById('navAuthBtn');
    const navLinks = document.getElementById('navLinks');
    if (!authBtn || !navLinks) return;

    const auth = await checkAuth();
    const currentPath = window.location.pathname;

    // Remove any existing dynamic dashboard/admin links
    navLinks.querySelectorAll('.dynamic-nav-link').forEach(el => el.remove());

    if (auth.authenticated) {
        // Insert Dashboard link before the CTA
        const ctaLi = authBtn.closest('li');

        const dashLi = document.createElement('li');
        dashLi.classList.add('dynamic-nav-link');
        const dashLink = document.createElement('a');
        dashLink.href = '/dashboard.html';
        dashLink.textContent = 'Dashboard';
        if (currentPath === '/dashboard.html') dashLink.style.color = 'var(--color-primary)';
        dashLi.appendChild(dashLink);
        navLinks.insertBefore(dashLi, ctaLi);

        // Wishlist link
        const wishLi = document.createElement('li');
        wishLi.classList.add('dynamic-nav-link');
        const wishLink = document.createElement('a');
        wishLink.href = '/wishlist.html';
        wishLink.textContent = '♡';
        wishLink.title = 'Wishlist';
        wishLink.style.fontSize = '1.25rem';
        if (currentPath === '/wishlist.html') wishLink.style.color = 'var(--color-primary)';
        wishLi.appendChild(wishLink);
        navLinks.insertBefore(wishLi, ctaLi);

        // Cart link with badge
        const cartLi = document.createElement('li');
        cartLi.classList.add('dynamic-nav-link');
        cartLi.style.position = 'relative';
        const cartLink = document.createElement('a');
        cartLink.href = '/cart.html';
        cartLink.innerHTML = '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="vertical-align:-4px"><circle cx="9" cy="21" r="1"/><circle cx="20" cy="21" r="1"/><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"/></svg>';
        cartLink.title = 'Cart';
        if (currentPath === '/cart.html') cartLink.style.color = 'var(--color-primary)';
        cartLi.appendChild(cartLink);
        navLinks.insertBefore(cartLi, ctaLi);

        // Fetch cart count
        try {
            const cartData = await API.get('/api/cart/count');
            if (cartData.count > 0) {
                const badge = document.createElement('span');
                badge.textContent = cartData.count;
                badge.style.cssText = 'position:absolute;top:-6px;right:-8px;background:var(--color-primary);color:#fff;font-size:0.65rem;min-width:16px;height:16px;border-radius:999px;display:flex;align-items:center;justify-content:center;font-weight:700;';
                cartLi.appendChild(badge);
            }
        } catch { }

        // If admin, also add Admin Panel link
        if (auth.role === 'Admin') {
            const adminLi = document.createElement('li');
            adminLi.classList.add('dynamic-nav-link');
            const adminLink = document.createElement('a');
            adminLink.href = '/admin.html';
            adminLink.textContent = 'Admin';
            if (currentPath === '/admin.html') adminLink.style.color = 'var(--color-primary)';
            adminLi.appendChild(adminLink);
            navLinks.insertBefore(adminLi, ctaLi);
        }

        // Change CTA to Logout
        authBtn.textContent = 'Logout';
        authBtn.href = '#';
        authBtn.onclick = (e) => { e.preventDefault(); logout(); };
    }
}

// ========== NAVBAR SCROLL EFFECT ==========
function initNavbar() {
    const navbar = document.getElementById('navbar');
    if (!navbar) return;

    window.addEventListener('scroll', () => {
        if (window.scrollY > 50) {
            navbar.classList.add('scrolled');
        } else {
            navbar.classList.remove('scrolled');
        }
    });

    // Mobile toggle
    const toggle = document.getElementById('navToggle');
    const links = document.getElementById('navLinks');
    if (toggle && links) {
        toggle.addEventListener('click', () => {
            links.classList.toggle('open');
            toggle.textContent = links.classList.contains('open') ? '✕' : '☰';
        });

        // Close on link click
        links.querySelectorAll('a').forEach(a => {
            a.addEventListener('click', () => {
                links.classList.remove('open');
                toggle.textContent = '☰';
            });
        });
    }
}

// ========== GSAP ANIMATIONS ==========
function initAnimations() {
    if (typeof gsap === 'undefined') return;

    // Register ScrollTrigger if available
    if (typeof ScrollTrigger !== 'undefined') {
        gsap.registerPlugin(ScrollTrigger);
    }

    // Page load animations
    gsap.from('.navbar', {
        y: -80,
        opacity: 0,
        duration: 1,
        ease: 'power3.out'
    });

    // Reveal on scroll — fast, no stagger delay
    const reveals = document.querySelectorAll('.reveal');
    if (reveals.length > 0 && typeof ScrollTrigger !== 'undefined') {
        reveals.forEach((el) => {
            gsap.fromTo(el,
                { opacity: 0, y: 20 },
                {
                    opacity: 1,
                    y: 0,
                    duration: 0.5,
                    ease: 'power2.out',
                    scrollTrigger: {
                        trigger: el,
                        start: 'top 92%',
                        toggleActions: 'play none none none'
                    }
                }
            );
        });
    } else {
        // Fallback without ScrollTrigger — show immediately
        reveals.forEach(el => {
            el.style.opacity = 1;
            el.style.transform = 'none';
        });
    }

    // Hero text stagger
    const heroOverlay = document.querySelector('.hero-overlay');
    if (heroOverlay) {
        gsap.from('.hero-overlay > *', {
            opacity: 0,
            y: 40,
            stagger: 0.15,
            duration: 1,
            ease: 'power3.out',
            delay: 0.3
        });
    }

    // Magnetic buttons effect
    document.querySelectorAll('.btn-primary, .nav-cta').forEach(btn => {
        btn.addEventListener('mousemove', (e) => {
            const rect = btn.getBoundingClientRect();
            const x = e.clientX - rect.left - rect.width / 2;
            const y = e.clientY - rect.top - rect.height / 2;
            gsap.to(btn, {
                x: x * 0.15,
                y: y * 0.15,
                duration: 0.3,
                ease: 'power2.out'
            });
        });
        btn.addEventListener('mouseleave', () => {
            gsap.to(btn, { x: 0, y: 0, duration: 0.5, ease: 'elastic.out(1, 0.3)' });
        });
    });

    // Stats count-up animation
    const statNumbers = document.querySelectorAll('.stat-number[data-target]');
    statNumbers.forEach(el => {
        const target = parseFloat(el.dataset.target);
        const suffix = el.dataset.suffix || '';
        const isDecimal = el.dataset.decimal === 'true';

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const obj = { val: 0 };
                    gsap.to(obj, {
                        val: target,
                        duration: 2,
                        ease: 'power2.out',
                        onUpdate: () => {
                            if (isDecimal) {
                                el.textContent = obj.val.toFixed(1) + suffix;
                            } else if (target >= 1000000) {
                                el.textContent = (obj.val / 1000000).toFixed(0) + 'M' + suffix;
                            } else {
                                el.textContent = Math.round(obj.val) + suffix;
                            }
                        }
                    });
                    observer.unobserve(el);
                }
            });
        }, { threshold: 0.5 });

        observer.observe(el);
    });
}



// ========== PRODUCTS PAGE ==========
async function initProductsPage() {
    const grid = document.getElementById('productsGrid');
    if (!grid) return;

    try {
        const products = await API.get('/api/product');
        renderProducts(products, grid);
        initFilterBar(products, grid);
    } catch (err) {
        grid.innerHTML = `<div class="text-center" style="grid-column:1/-1;"><p style="color:var(--color-danger);">Failed to load products. Please try again.</p></div>`;
    }
}

function renderProducts(products, container) {
    if (products.length === 0) {
        container.innerHTML = `<div class="text-center" style="grid-column:1/-1;"><p style="color:var(--text-secondary);">No products found.</p></div>`;
        return;
    }

    container.innerHTML = products.map(p => `
        <div class="product-card" onclick="viewProduct(${p.id})">
            <div class="product-image-container">
                ${p.imageUrl && p.imageUrl !== '' && p.imageUrl !== '/images/product.jpg'
            ? `<img class="product-image" src="${p.imageUrl}" alt="${p.name}" style="height:240px;width:100%;object-fit:cover;">`
            : `<div class="product-image" style="height:240px;background:linear-gradient(135deg, rgba(79,142,247,0.1), rgba(124,92,252,0.1));display:flex;align-items:center;justify-content:center;">
                        ${getCategoryIcon(p.category)}
                    </div>`
        }
            </div>
            <div class="product-info">
                <div class="product-category">${p.category}</div>
                <div class="product-name">${p.name}</div>
                <div class="product-desc">${p.description}</div>
                <div class="product-price">$${p.price.toLocaleString('en-US', { minimumFractionDigits: 2 })}</div>
                <div class="product-actions">
                    <a href="/product-detail.html?id=${p.id}" class="btn btn-primary btn-sm">Learn More</a>
                    <button class="btn btn-outline btn-sm" onclick="event.stopPropagation();addToCart(${p.id})">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="vertical-align:-2px"><circle cx="9" cy="21" r="1"/><circle cx="20" cy="21" r="1"/><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"/></svg>
                        Cart
                    </button>
                    <button class="btn btn-glass btn-sm" onclick="event.stopPropagation();addToWishlist(${p.id})">♡</button>
                </div>
            </div>
        </div>
    `).join('');

    // Animate cards in
    if (typeof gsap !== 'undefined') {
        gsap.from('.product-card', {
            opacity: 0,
            y: 30,
            stagger: 0.1,
            duration: 0.6,
            ease: 'power3.out'
        });
    }
}

function getCategoryIcon(category) {
    const icons = {
        'Hardware': '<svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="var(--color-primary)" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><rect x="4" y="4" width="16" height="16" rx="2" ry="2"/><rect x="9" y="9" width="6" height="6"/><line x1="9" y1="1" x2="9" y2="4"/><line x1="15" y1="1" x2="15" y2="4"/><line x1="9" y1="20" x2="9" y2="23"/><line x1="15" y1="20" x2="15" y2="23"/><line x1="20" y1="9" x2="23" y2="9"/><line x1="20" y1="14" x2="23" y2="14"/><line x1="1" y1="9" x2="4" y2="9"/><line x1="1" y1="14" x2="4" y2="14"/></svg>',
        'Software': '<svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="var(--color-accent)" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="16 18 22 12 16 6"/><polyline points="8 6 2 12 8 18"/></svg>',
        'Service': '<svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="var(--color-primary)" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>'
    };
    return icons[category] || '<svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="var(--color-primary)" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"/><polyline points="3.27 6.96 12 12.01 20.73 6.96"/><line x1="12" y1="22.08" x2="12" y2="12"/></svg>';
}

function initFilterBar(allProducts, container) {
    const filterBtns = document.querySelectorAll('.filter-btn');
    filterBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            filterBtns.forEach(b => b.classList.remove('active'));
            btn.classList.add('active');

            const cat = btn.dataset.category;
            const filtered = cat === 'all' ? allProducts : allProducts.filter(p => p.category === cat);
            renderProducts(filtered, container);
        });
    });

    // Check URL for category
    const urlParams = new URLSearchParams(window.location.search);
    const urlCat = urlParams.get('category');
    if (urlCat) {
        filterBtns.forEach(b => b.classList.remove('active'));
        const matchBtn = Array.from(filterBtns).find(b => b.dataset.category === urlCat);
        if (matchBtn) {
            matchBtn.classList.add('active');
            const filtered = allProducts.filter(p => p.category === urlCat);
            renderProducts(filtered, container);
        }
    }
}

function viewProduct(id) {
    window.location.href = `/product-detail.html?id=${id}`;
}

// ========== PRODUCT DETAIL PAGE ==========
async function initProductDetail() {
    const container = document.getElementById('productDetail');
    if (!container) return;

    const params = new URLSearchParams(window.location.search);
    const id = params.get('id');
    if (!id) {
        container.innerHTML = `<div class="text-center" style="grid-column:1/-1;"><p style="color:var(--color-danger);">No product ID specified.</p><a href="/products.html" class="btn btn-outline mt-2">Back to Products</a></div>`;
        return;
    }

    try {
        const product = await API.get(`/api/product/${id}`);
        document.title = `${product.name} — LuminaTech`;

        container.innerHTML = `
            <div>
                ${product.imageUrl && product.imageUrl !== '' && product.imageUrl !== '/images/product.jpg'
                ? `<img class="product-detail-image" src="${product.imageUrl}" alt="${product.name}" style="width:100%;border-radius:var(--radius-lg);aspect-ratio:4/3;object-fit:cover;">`
                : `<div class="product-detail-image" style="background:linear-gradient(135deg, rgba(79,142,247,0.1), rgba(124,92,252,0.1));display:flex;align-items:center;justify-content:center;font-size:6rem;border-radius:var(--radius-lg);">
                        ${getCategoryIcon(product.category)}
                    </div>`
            }
            </div>
            <div class="product-detail-info">
                <span class="label">${product.category}</span>
                <h1 class="mt-1">${product.name}</h1>
                <div class="price">$${product.price.toLocaleString('en-US', { minimumFractionDigits: 2 })}</div>
                <p style="color:var(--text-secondary);font-size:1.0625rem;line-height:1.8;">${product.description}</p>

                <div class="flex gap-2 mt-3" style="flex-wrap:wrap;">
                    <button class="btn btn-primary" onclick="addToCart(${product.id})">
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="vertical-align:-3px"><circle cx="9" cy="21" r="1"/><circle cx="20" cy="21" r="1"/><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"/></svg>
                        Add to Cart
                    </button>
                    <button class="btn btn-outline" onclick="addToWishlist(${product.id})">♡ Wishlist</button>
                    <a href="/contact.html" class="btn btn-glass">Request Demo</a>
                </div>

                <table class="specs-table mt-4">
                    <tr><td>Category</td><td>${product.category}</td></tr>
                    <tr><td>Status</td><td><span class="badge badge-active">Available</span></td></tr>
                    <tr><td>Product ID</td><td style="font-family:var(--font-mono);">#LT-${String(product.id).padStart(4, '0')}</td></tr>
                    <tr><td>Warranty</td><td>2 Years Standard</td></tr>
                    <tr><td>Support</td><td>24/7 Priority Support</td></tr>
                </table>
            </div>
        `;

        // Load related products
        await loadRelatedProducts(product);

    } catch (err) {
        container.innerHTML = `<div class="text-center" style="grid-column:1/-1;"><p style="color:var(--color-danger);">Product not found.</p><a href="/products.html" class="btn btn-outline mt-2">Back to Products</a></div>`;
    }
}

async function loadRelatedProducts(currentProduct) {
    try {
        const products = await API.get('/api/product');
        const related = products.filter(p => p.id !== currentProduct.id).slice(0, 4);
        const container = document.getElementById('relatedProducts');
        const section = document.getElementById('relatedSection');

        if (related.length > 0 && container && section) {
            section.style.display = 'block';
            container.innerHTML = related.map(p => `
                <div class="product-card" style="min-width:300px;flex-shrink:0;" onclick="viewProduct(${p.id})">
                    <div class="product-image-container">
                        ${p.imageUrl && p.imageUrl !== '' && p.imageUrl !== '/images/product.jpg'
                    ? `<img src="${p.imageUrl}" alt="${p.name}" style="height:160px;width:100%;object-fit:cover;">`
                    : `<div style="height:160px;background:linear-gradient(135deg, rgba(79,142,247,0.08), rgba(124,92,252,0.08));display:flex;align-items:center;justify-content:center;">
                                ${getCategoryIcon(p.category)}
                            </div>`
                }
                    </div>
                    <div class="product-info">
                        <div class="product-category">${p.category}</div>
                        <div class="product-name">${p.name}</div>
                        <div class="product-price">$${p.price.toLocaleString('en-US', { minimumFractionDigits: 2 })}</div>
                    </div>
                </div>
            `).join('');
        }
    } catch { }
}

// ========== WISHLIST ==========
async function addToWishlist(productId) {
    try {
        const auth = await checkAuth();
        if (!auth.authenticated) {
            window.location.href = '/login.html';
            return;
        }
        await API.post('/api/wishlist', { productId });
        showToast('Added to wishlist! ♡', 'success');
    } catch (err) {
        showToast('Could not add to wishlist', 'error');
    }
}

async function addToCart(productId) {
    try {
        const auth = await checkAuth();
        if (!auth.authenticated) {
            window.location.href = '/login.html';
            return;
        }
        await API.post('/api/cart', { productId, quantity: 1 });
        showToast('Added to cart!', 'success');
        updateNavAuth(); // refresh cart badge
    } catch (err) {
        showToast('Could not add to cart', 'error');
    }
}

// ========== DASHBOARD PAGE ==========
async function initDashboard() {
    const header = document.getElementById('dashboardHeader');
    if (!header) return;

    const auth = await checkAuth();
    if (!auth.authenticated) {
        window.location.href = '/login.html';
        return;
    }

    // Update profile info
    document.getElementById('userName').textContent = auth.name || 'User';
    document.getElementById('userEmail').textContent = auth.email || '';
    document.getElementById('profileName').textContent = auth.name || '—';
    document.getElementById('profileEmail').textContent = auth.email || '—';
    document.getElementById('profileRole').textContent = auth.role || '—';
    document.getElementById('profileDate').textContent = 'Member';

    const roleEl = document.getElementById('userRole');
    if (auth.role === 'Admin') {
        roleEl.textContent = 'Admin';
        roleEl.className = 'badge badge-admin';
    }

    // Profile pic
    const picEl = document.getElementById('profilePic');
    if (auth.profilePicture && auth.profilePicture !== '') {
        picEl.outerHTML = `<img src="${auth.profilePicture}" class="profile-pic" id="profilePic" alt="Profile">`;
    } else {
        picEl.textContent = (auth.name || 'U')[0].toUpperCase();
    }

    // Load wishlist
    loadWishlist();
}

async function loadWishlist() {
    const container = document.getElementById('wishlistGrid');
    if (!container) return;

    try {
        const items = await API.get('/api/users/me/wishlist');
        if (items.length === 0) {
            container.innerHTML = `
                <div class="text-center" style="grid-column:1/-1;">
                    <div class="glass-card" style="padding:3rem;">
                        <p style="color:var(--text-secondary);">Your wishlist is empty</p>
                        <a href="/products.html" class="btn btn-outline btn-sm mt-2">Browse Products</a>
                    </div>
                </div>`;
            return;
        }

        container.innerHTML = items.map(item => `
            <div class="product-card">
                <div class="product-info">
                    <div class="product-category">${item.category}</div>
                    <div class="product-name">${item.name}</div>
                    <div class="product-desc">${item.description}</div>
                    <div class="product-price">$${item.price.toLocaleString('en-US', { minimumFractionDigits: 2 })}</div>
                    <div class="product-actions">
                        <a href="/product-detail.html?id=${item.id}" class="btn btn-primary btn-sm">View</a>
                        <button class="btn btn-danger btn-sm" onclick="removeFromWishlist(${item.id})">Remove</button>
                    </div>
                </div>
            </div>
        `).join('');
    } catch {
        container.innerHTML = `<div class="text-center" style="grid-column:1/-1;"><p style="color:var(--text-secondary);">Could not load wishlist.</p></div>`;
    }
}

async function removeFromWishlist(productId) {
    try {
        await API.delete(`/api/users/me/wishlist/${productId}`);
        loadWishlist();
        showToast('Removed from wishlist', 'success');
    } catch {
        showToast('Could not remove item', 'error');
    }
}

function switchTab(tabName) {
    // Update tab buttons
    document.querySelectorAll('.dashboard-tab').forEach(tab => {
        tab.classList.toggle('active', tab.dataset.tab === tabName);
    });

    // Update tab content
    document.querySelectorAll('.tab-content').forEach(content => {
        content.classList.add('hidden');
    });
    const targetContent = document.getElementById(`tab-${tabName}`);
    if (targetContent) targetContent.classList.remove('hidden');
}

// ========== CONTACT FORM ==========
async function submitContactForm(event) {
    event.preventDefault();

    const btn = document.getElementById('contactSubmitBtn');
    btn.textContent = 'Sending...';
    btn.disabled = true;

    const data = {
        fullName: document.getElementById('contactName').value,
        email: document.getElementById('contactEmail').value,
        company: document.getElementById('contactCompany').value,
        productInterest: document.getElementById('contactInterest').value,
        message: document.getElementById('contactMessage').value
    };

    try {
        await API.post('/api/contact', data);
        document.getElementById('contactFormCard').classList.add('hidden');
        document.getElementById('contactSuccess').classList.remove('hidden');
    } catch (err) {
        showToast('Failed to send message. Please try again.', 'error');
        btn.textContent = 'Send Message →';
        btn.disabled = false;
    }
}

function resetContactForm() {
    document.getElementById('contactForm').reset();
    document.getElementById('contactFormCard').classList.remove('hidden');
    document.getElementById('contactSuccess').classList.add('hidden');
    document.getElementById('contactSubmitBtn').textContent = 'Send Message →';
    document.getElementById('contactSubmitBtn').disabled = false;
}

// ========== ADMIN PANEL ==========
async function initAdmin() {
    const overviewEl = document.getElementById('adminOverview');
    if (!overviewEl) return;

    const auth = await checkAuth();
    if (!auth.authenticated || auth.role !== 'Admin') {
        document.body.innerHTML = `
            <div class="error-page">
                <div class="error-code">403</div>
                <h2>Access Denied</h2>
                <p>You don't have permission to access this page.</p>
                <a href="/" class="btn btn-outline">Go Home</a>
            </div>`;
        return;
    }

    loadAdminStats();
    loadAdminProducts();
    loadAdminUsers();
    loadAdminContacts();
}

function showAdminSection(section) {
    ['Overview', 'Products', 'Users', 'Contacts'].forEach(s => {
        const el = document.getElementById(`admin${s}`);
        const sideEl = document.getElementById(`side${s}`);
        if (el) el.classList.toggle('hidden', s.toLowerCase() !== section);
        if (sideEl) sideEl.classList.toggle('active', s.toLowerCase() === section);
    });
}

async function loadAdminStats() {
    try {
        const stats = await API.get('/api/admin/stats');
        document.getElementById('statUsers').textContent = stats.totalUsers;
        document.getElementById('statProducts').textContent = stats.totalProducts;
        document.getElementById('statContacts').textContent = stats.totalContacts;
        document.getElementById('statUnread').textContent = stats.unreadContacts;
    } catch { }
}

async function loadAdminProducts() {
    try {
        const products = await API.get('/api/product');
        const tbody = document.getElementById('productsTableBody');
        tbody.innerHTML = products.map(p => `
            <tr>
                <td>${p.id}</td>
                <td><strong>${p.name}</strong></td>
                <td><span class="badge badge-user">${p.category}</span></td>
                <td>$${p.price.toLocaleString('en-US', { minimumFractionDigits: 2 })}</td>
                <td><span class="badge ${p.isActive ? 'badge-active' : 'badge-inactive'}">${p.isActive ? 'Active' : 'Inactive'}</span></td>
                <td>
                    <button class="btn btn-outline btn-sm" onclick="editProduct(${p.id})">Edit</button>
                    <button class="btn btn-danger btn-sm" onclick="deleteProduct(${p.id})">Delete</button>
                </td>
            </tr>
        `).join('');
    } catch { }
}

async function loadAdminUsers() {
    try {
        const users = await API.get('/api/admin/users');
        const tbody = document.getElementById('usersTableBody');
        tbody.innerHTML = users.map(u => `
            <tr>
                <td>${u.id}</td>
                <td>${u.fullName}</td>
                <td>${u.email}</td>
                <td><span class="badge ${u.role === 'Admin' ? 'badge-admin' : 'badge-user'}">${u.role}</span></td>
                <td><span class="badge ${u.isActive ? 'badge-active' : 'badge-inactive'}">${u.isActive ? 'Active' : 'Inactive'}</span></td>
                <td>
                    <button class="btn btn-outline btn-sm" onclick="toggleRole(${u.id}, '${u.role}')">${u.role === 'Admin' ? 'Demote' : 'Promote'}</button>
                    <button class="btn btn-glass btn-sm" onclick="toggleUserActive(${u.id})">${u.isActive ? 'Deactivate' : 'Activate'}</button>
                </td>
            </tr>
        `).join('');
    } catch { }
}

async function loadAdminContacts() {
    try {
        const contacts = await API.get('/api/admin/contacts');
        const tbody = document.getElementById('contactsTableBody');
        if (contacts.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center" style="padding:2rem;color:var(--text-secondary);">No submissions yet</td></tr>';
            return;
        }
        tbody.innerHTML = contacts.map(c => `
            <tr>
                <td>${c.fullName}</td>
                <td>${c.email}</td>
                <td>${c.company || '—'}</td>
                <td>${c.productInterest || '—'}</td>
                <td><span class="badge ${c.isRead ? 'badge-read' : 'badge-unread'}">${c.isRead ? 'Read' : 'New'}</span></td>
                <td>
                    ${!c.isRead ? `<button class="btn btn-outline btn-sm" onclick="markAsRead(${c.id})">Mark Read</button>` : ''}
                </td>
            </tr>
        `).join('');
    } catch { }
}

// Admin actions
async function toggleRole(userId, currentRole) {
    const newRole = currentRole === 'Admin' ? 'User' : 'Admin';
    try {
        await API.put(`/api/admin/users/${userId}/role`, { role: newRole });
        loadAdminUsers();
        showToast(`Role updated to ${newRole}`, 'success');
    } catch {
        showToast('Failed to update role', 'error');
    }
}

async function toggleUserActive(userId) {
    try {
        await API.put(`/api/admin/users/${userId}/toggle`);
        loadAdminUsers();
        showToast('User status updated', 'success');
    } catch {
        showToast('Failed to update status', 'error');
    }
}

async function markAsRead(contactId) {
    try {
        await API.put(`/api/admin/contacts/${contactId}/read`);
        loadAdminContacts();
        loadAdminStats();
    } catch { }
}

// Product CRUD Modal
function openProductModal(product = null) {
    const modal = document.getElementById('productModal');
    const title = document.getElementById('productModalTitle');
    const preview = document.getElementById('imagePreview');
    const fileInput = document.getElementById('productImageFile');

    if (product) {
        title.textContent = 'Edit Product';
        document.getElementById('productId').value = product.id;
        document.getElementById('productName').value = product.name;
        document.getElementById('productDescription').value = product.description;
        document.getElementById('productPrice').value = product.price;
        document.getElementById('productCategory').value = product.category;
        document.getElementById('productImageUrl').value = product.imageUrl || '';
        // Show existing image preview
        if (preview && product.imageUrl && product.imageUrl !== '/images/product.jpg') {
            preview.innerHTML = `<img src="${product.imageUrl}" alt="Preview" style="width:100%;max-height:200px;object-fit:cover;border-radius:var(--radius-md);">`;
            preview.style.display = 'block';
        } else if (preview) {
            preview.innerHTML = '';
            preview.style.display = 'none';
        }
    } else {
        title.textContent = 'Add Product';
        document.getElementById('productForm').reset();
        document.getElementById('productId').value = '';
        if (preview) {
            preview.innerHTML = '';
            preview.style.display = 'none';
        }
    }
    if (fileInput) fileInput.value = '';

    modal.classList.add('active');
}

function closeProductModal() {
    document.getElementById('productModal').classList.remove('active');
}

async function editProduct(id) {
    try {
        const product = await API.get(`/api/product/${id}`);
        openProductModal(product);
    } catch {
        showToast('Failed to load product', 'error');
    }
}

async function saveProduct(event) {
    event.preventDefault();

    const id = document.getElementById('productId').value;
    const fileInput = document.getElementById('productImageFile');
    let imageUrl = document.getElementById('productImageUrl').value || '/images/product.jpg';
    let cloudinaryPublicId = '';

    // Upload image to Cloudinary if file selected
    if (fileInput && fileInput.files && fileInput.files.length > 0) {
        const formData = new FormData();
        formData.append('file', fileInput.files[0]);

        try {
            showToast('Uploading image...', 'info');
            const uploadRes = await fetch('/api/media/upload', {
                method: 'POST',
                body: formData
            });

            if (!uploadRes.ok) {
                const err = await uploadRes.json();
                showToast(err.message || 'Image upload failed', 'error');
                return;
            }

            const uploadData = await uploadRes.json();
            imageUrl = uploadData.url;
            cloudinaryPublicId = uploadData.publicId;
        } catch (err) {
            showToast('Image upload failed: ' + err.message, 'error');
            return;
        }
    }

    const data = {
        name: document.getElementById('productName').value,
        description: document.getElementById('productDescription').value,
        price: parseFloat(document.getElementById('productPrice').value),
        category: document.getElementById('productCategory').value,
        imageUrl: imageUrl,
        cloudinaryPublicId: cloudinaryPublicId
    };

    try {
        if (id) {
            await API.put(`/api/product/${id}`, data);
        } else {
            await API.post('/api/product', data);
        }
        closeProductModal();
        loadAdminProducts();
        loadAdminStats();
        showToast(id ? 'Product updated' : 'Product created', 'success');
    } catch {
        showToast('Failed to save product', 'error');
    }
}

async function deleteProduct(id) {
    if (!confirm('Are you sure you want to delete this product?')) return;
    try {
        await API.delete(`/api/product/${id}`);
        loadAdminProducts();
        loadAdminStats();
        showToast('Product deleted', 'success');
    } catch {
        showToast('Failed to delete product', 'error');
    }
}

// ========== IMAGE PREVIEW ==========
function handleImagePreview() {
    const fileInput = document.getElementById('productImageFile');
    const preview = document.getElementById('imagePreview');
    if (!fileInput || !preview || !fileInput.files || fileInput.files.length === 0) return;

    const file = fileInput.files[0];
    const reader = new FileReader();
    reader.onload = (e) => {
        preview.innerHTML = `
            <div style="position:relative;display:inline-block;">
                <img src="${e.target.result}" alt="Preview" style="width:100%;max-height:200px;object-fit:cover;border-radius:var(--radius-md);">
                <button type="button" onclick="clearImagePreview()" style="position:absolute;top:0.5rem;right:0.5rem;background:rgba(0,0,0,0.6);color:white;border:none;border-radius:50%;width:24px;height:24px;cursor:pointer;font-size:0.75rem;display:flex;align-items:center;justify-content:center;">✕</button>
            </div>`;
        preview.style.display = 'block';
    };
    reader.readAsDataURL(file);
}

function clearImagePreview() {
    const fileInput = document.getElementById('productImageFile');
    const preview = document.getElementById('imagePreview');
    if (fileInput) fileInput.value = '';
    if (preview) {
        preview.innerHTML = '';
        preview.style.display = 'none';
    }
}

// ========== TOAST NOTIFICATIONS ==========
function showToast(message, type = 'info') {
    const toast = document.createElement('div');
    toast.style.cssText = `
        position: fixed;
        bottom: 2rem;
        right: 2rem;
        padding: 0.875rem 1.5rem;
        background: ${type === 'success' ? 'rgba(0,200,150,0.15)' : type === 'error' ? 'rgba(255,77,106,0.15)' : 'rgba(79,142,247,0.15)'};
        border: 1px solid ${type === 'success' ? 'rgba(0,200,150,0.3)' : type === 'error' ? 'rgba(255,77,106,0.3)' : 'rgba(79,142,247,0.3)'};
        border-radius: 12px;
        color: ${type === 'success' ? '#00C896' : type === 'error' ? '#FF4D6A' : '#4F8EF7'};
        font-size: 0.9375rem;
        font-weight: 500;
        backdrop-filter: blur(12px);
        z-index: 9999;
        transform: translateY(20px);
        opacity: 0;
        transition: all 0.3s ease;
    `;
    toast.textContent = message;
    document.body.appendChild(toast);

    requestAnimationFrame(() => {
        toast.style.transform = 'translateY(0)';
        toast.style.opacity = '1';
    });

    setTimeout(() => {
        toast.style.transform = 'translateY(20px)';
        toast.style.opacity = '0';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// ========== INITIALIZATION ==========
document.addEventListener('DOMContentLoaded', () => {
    initNavbar();
    initAnimations();
    updateNavAuth();

    // Page-specific initialization
    const path = window.location.pathname;

    if (path === '/products.html') {
        initProductsPage();
    } else if (path === '/product-detail.html') {
        initProductDetail();
    } else if (path === '/dashboard.html') {
        initDashboard();
    } else if (path === '/admin.html') {
        initAdmin();
    }
});

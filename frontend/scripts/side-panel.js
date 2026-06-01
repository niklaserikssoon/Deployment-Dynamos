document.addEventListener('DOMContentLoaded', () => {
  const menuToggle = document.getElementById('menu-toggle');
  const sidePanel = document.getElementById('side-panel');
  const backdrop = document.getElementById('side-panel-backdrop');
  const authStatusEl = document.getElementById('side-auth-status');

  if (!menuToggle || !sidePanel || !backdrop) {
    console.warn('Sidebar elements missing from DOM');
    return;
  }

  let isOpen = false;

  function setPanelFocusable(enabled) {
    sidePanel.querySelectorAll('input, button, select, textarea, a[href], [tabindex]').forEach(el => {
      el.disabled = !enabled;
      el.setAttribute('tabindex', enabled ? '0' : '-1');
    });
  }

  setPanelFocusable(false);

  function openPanel() {
    if (isOpen) return;
    isOpen = true;

    sidePanel.classList.add('open');
    backdrop.classList.add('visible');
    menuToggle.classList.add('is-open');

    sidePanel.hidden = false;
    sidePanel.setAttribute('aria-hidden', 'false');
    menuToggle.setAttribute('aria-expanded', 'true');

    setPanelFocusable(true);

    const firstFocusable = sidePanel.querySelector('input, button, [tabindex]');
    firstFocusable?.focus();
  }

  function closePanel() {
    if (!isOpen) return;
    isOpen = false;

    sidePanel.classList.remove('open');
    backdrop.classList.remove('visible');
    menuToggle.classList.remove('is-open');

    sidePanel.hidden = true;
    sidePanel.setAttribute('aria-hidden', 'true');
    menuToggle.setAttribute('aria-expanded', 'false');

    setPanelFocusable(false);
  }

  menuToggle.addEventListener('click', () => isOpen ? closePanel() : openPanel());
  backdrop.addEventListener('click', closePanel);
  document.addEventListener('keydown', (e) => { if (e.key === 'Escape') closePanel(); });

  // PATH LOGIC
  const isInSrc = location.pathname.includes('/src/');
  const loginPath = isInSrc ? 'login.html' : './src/login.html';
  const profilePath = isInSrc ? 'profile.html' : './src/profile.html';
  const exerciseBankPath = isInSrc ? 'exerciseBank.html' : './src/exerciseBank.html';

  // UPDATE AUTH UI
  function updateAuthStatus() {
    const current = localStorage.getItem('currentUser');

    const loginBtn = sidePanel.querySelector('[data-nav="log-in"]');
    const registerBtn = sidePanel.querySelector('[data-nav="register"]');
    const profileBtn = sidePanel.querySelector('[data-nav="profile"]');

    // Update "Logged in as"
    if (authStatusEl) {
        authStatusEl.textContent = current ? `Logged in as: ${current}` : '';
    }

    if (current) {
        // Show My Profile
        if (profileBtn) profileBtn.hidden = false;

        // Change Log In → Log Out
        if (loginBtn) loginBtn.textContent = 'Log Out';

        // Hide Register
        if (registerBtn) registerBtn.style.display = 'none';

        // Move My Profile ABOVE Log Out
        const list = sidePanel.querySelector('.side-panel-list');
        if (list && profileBtn && loginBtn) {
            list.insertBefore(profileBtn.closest('li'), loginBtn.closest('li'));
        }

    } else {
        // Hide My Profile
        if (profileBtn) profileBtn.hidden = true;

        // Reset Log In
        if (loginBtn) loginBtn.textContent = 'Log In';

        // Show Register
        if (registerBtn) registerBtn.style.display = '';
    }
}

  // SIDEBAR NAVIGATION
sidePanel.addEventListener('click', (e) => {
    const btn = e.target.closest('[data-nav]');
    if (!btn) return;

    const nav = btn.dataset.nav;
    sessionStorage.setItem('activeView', nav);

    switch (nav) {

        case 'show-exercises':
            window.location.href = `${exerciseBankPath}#show-all-exercises`;
            break;

        case 'search-exercises': {
            const input = document.getElementById('side-panel-search');
            const query = input?.value.trim();
            const url = query
                ? `${exerciseBankPath}#search=${encodeURIComponent(query)}`
                : `${exerciseBankPath}#search`;
            window.location.href = url;
            break;
        }

        case 'profile': {
            // NEW PROFILE PAGE
            window.location.href = isInSrc
                ? 'profile.html'
                : './src/profile.html';
            break;
        }

        case 'log-in':
        case 'login': {
            const current = localStorage.getItem('currentUser');

            if (current) {
                // LOG OUT
                localStorage.removeItem('currentUser');
                updateAuthStatus();
                closePanel();
                return;
            }

            // Go to login page
            window.location.href = `${loginPath}#login`;
            break;
        }

        case 'register':
            window.location.href = `${loginPath}#register`;
            break;

        default:
            break;
    }

    closePanel();
});

  // Restore state
  const savedView = sessionStorage.getItem('activeView');
  if (savedView === 'add' && typeof window.loadComponent === 'function') {
    window.loadComponent().then(() => {
      document.getElementById('create-item-form')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
  }

  updateAuthStatus();
  window.addEventListener('storage', (e) => { if (e.key === 'currentUser') updateAuthStatus(); });

  sessionStorage.removeItem('activeView');
});

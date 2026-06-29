document.addEventListener("DOMContentLoaded", function() {
    const profileArea = document.getElementById('member-profile-area');
    const bigAvatar = document.getElementById('active-big-avatar');
    const nameEl = document.getElementById('active-member-name');
    const roleEl = document.getElementById('active-member-role');
    const descEl = document.getElementById('active-member-desc');
    const smallItems = document.querySelectorAll('.small-avatar-item');
    const container = document.querySelector('.team-card');
    
    let activeIndex = 0;
    let autoPlayTimer = null;

    function setMember(index) {
        activeIndex = index;
        const targetItem = smallItems[index];
        const wrapper = document.querySelector('.big-avatar-wrapper');
        const detailsCol = document.querySelector('.member-details-col');

        wrapper.classList.add('changing');
        detailsCol.classList.add('changing');

        setTimeout(() => {
            // Update Text & Source Image
            const img = targetItem.querySelector('img');
            if (img) {
                bigAvatar.src = img.src;
            }
            nameEl.textContent = targetItem.getAttribute('data-name');
            roleEl.textContent = targetItem.getAttribute('data-role');
            descEl.textContent = targetItem.getAttribute('data-desc');

            // Apply Level Border Styling
            const color = targetItem.getAttribute('data-color');
            const glow = targetItem.getAttribute('data-glow');
            wrapper.style.borderColor = color;
            wrapper.style.boxShadow = `0 10px 30px ${glow}`;

            // Update Active Class on Dots Bar
            smallItems.forEach(i => {
                i.classList.remove('active');
                i.style.borderColor = 'rgba(255,255,255,0.2)';
            });
            targetItem.classList.add('active');
            targetItem.style.borderColor = color;

            wrapper.classList.remove('changing');
            detailsCol.classList.remove('changing');
        }, 220);
    }

    smallItems.forEach((item, idx) => {
        item.addEventListener('click', () => {
            setMember(idx);
            startAutoPlay(); // Resets timer to fresh 3 seconds for the newly active member
        });
    });

    function startAutoPlay() {
        if (autoPlayTimer) {
            clearInterval(autoPlayTimer);
        }
        autoPlayTimer = setInterval(() => {
            let nextIndex = (activeIndex + 1) % smallItems.length;
            setMember(nextIndex);
        }, 3000);
    }

    container.addEventListener('mouseenter', () => {
        if (autoPlayTimer) {
            clearInterval(autoPlayTimer);
            autoPlayTimer = null;
        }
    });
    container.addEventListener('mouseleave', startAutoPlay);

    // Initial setup and sync for the first item on load
    if (smallItems.length > 0) {
        const firstColor = smallItems[0].getAttribute('data-color');
        smallItems[0].style.borderColor = firstColor;

        // Sync the big avatar details dynamically with the first small item on page load
        const img = smallItems[0].querySelector('img');
        if (img) {
            bigAvatar.src = img.src;
        }
        nameEl.textContent = smallItems[0].getAttribute('data-name');
        roleEl.textContent = smallItems[0].getAttribute('data-role');
        descEl.textContent = smallItems[0].getAttribute('data-desc');

        const glow = smallItems[0].getAttribute('data-glow');
        const wrapper = document.querySelector('.big-avatar-wrapper');
        if (wrapper) {
            wrapper.style.borderColor = firstColor;
            wrapper.style.boxShadow = `0 10px 30px ${glow}`;
        }
    }

    startAutoPlay();
});

/**
 * Food Route - Swipe Functionality
 * Single-card swipe flow with clean handoff between cards.
 */

class SwipeCards {
    constructor() {
        this.cardStack = document.getElementById('card-stack');
        this.likeBtn = document.getElementById('like-btn');
        this.nopeBtn = document.getElementById('nope-btn');
        this.counterValue = document.getElementById('counter-value');

        if (!this.cardStack) return;

        this.cards = Array.from(this.cardStack.querySelectorAll('.swipe-card'));
        this.currentCardIndex = this.cards.length - 1;

        this.dragState = null;
        this.gestureCleanup = null;
        this.isAnimating = false;

        this.minimumSwipeDistance = 100;
        this.velocityThreshold = 0.55;
        this.maxRotation = 12;
        this.enterDuration = 280;
        this.exitDuration = 320;

        this.init();
    }

    init() {
        this.setupCards(false);
        this.bindButtons();
        this.bindKeyboard();
    }

    get topCard() {
        return this.cards[this.currentCardIndex] ?? null;
    }

    setupCards(animateTopCard = false) {
        if (!this.cardStack) return;

        this.teardownGestureEvents();

        this.cards.forEach((card, index) => {
            const isTopCard = index === this.currentCardIndex;
            const isNextCard = index === this.currentCardIndex - 1;

            card.classList.remove('is-dragging', 'is-returning', 'is-swiping', 'is-active', 'is-next');
            card.setAttribute('draggable', 'false');
            card.querySelectorAll('img').forEach((image) => {
                image.setAttribute('draggable', 'false');
            });
            card.ondragstart = this.preventNativeDrag;

            if (isNextCard) {
                card.classList.add('is-next');
                card.style.zIndex = '1';
                card.style.transition = 'transform 220ms ease-out, opacity 220ms ease-out';
                card.style.opacity = '0';
                card.style.transform = 'translate3d(0, 24px, 0) scale(0.98)';
                this.resetIndicators(card);
                return;
            }

            if (!isTopCard) {
                card.style.transition = 'none';
                card.style.zIndex = '0';
                card.style.opacity = '1';
                card.style.transform = 'translate3d(0, 0, 0) rotate(0deg)';
                this.resetIndicators(card);
                return;
            }

            card.classList.add('is-active');
            card.style.zIndex = '2';
            this.resetIndicators(card);

            if (animateTopCard) {
                card.style.transition = 'none';
                card.style.opacity = '0';
                card.style.transform = 'translate3d(0, 30px, 0) rotate(0deg)';

                requestAnimationFrame(() => {
                    requestAnimationFrame(() => {
                        card.style.transition = `transform ${this.enterDuration}ms ease-out, opacity ${this.enterDuration}ms ease-out`;
                        card.style.opacity = '1';
                        card.style.transform = 'translate3d(0, 0, 0) rotate(0deg)';
                    });
                });
            } else {
                card.style.transition = 'none';
                card.style.opacity = '1';
                card.style.transform = 'translate3d(0, 0, 0) rotate(0deg)';
            }
        });

        if (this.topCard) {
            this.bindGestureEvents(this.topCard);
        }
    }

    bindGestureEvents(card) {
        const onPointerDown = (event) => {
            if (!event.isPrimary) return;
            if (event.button !== undefined && event.button !== 0) return;
            if (this.topCard !== card || card.classList.contains('is-swiping') || this.isAnimating) return;

            const now = performance.now();

            this.dragState = {
                card,
                pointerId: event.pointerId,
                startX: event.clientX,
                startY: event.clientY,
                deltaX: 0,
                deltaY: 0,
                rawDeltaY: 0,
                lastX: event.clientX,
                lastTime: now,
                velocityX: 0
            };

            card.classList.add('is-dragging');
            card.classList.remove('is-returning');
            card.style.transition = 'none';
            card.style.opacity = '1';
            card.style.transform = 'translate3d(0, 0, 0) rotate(0deg)';

            if (typeof card.setPointerCapture === 'function') {
                card.setPointerCapture(event.pointerId);
            }

            if (window.getSelection) {
                window.getSelection().removeAllRanges();
            }

            event.preventDefault();
        };

        const onPointerMove = (event) => {
            if (!this.dragState || event.pointerId !== this.dragState.pointerId) return;

            const now = performance.now();
            const rawDeltaY = event.clientY - this.dragState.startY;
            const deltaX = event.clientX - this.dragState.startX;
            const deltaY = rawDeltaY * 0.18;
            const deltaTime = Math.max(now - this.dragState.lastTime, 16);

            this.dragState.deltaX = deltaX;
            this.dragState.deltaY = deltaY;
            this.dragState.rawDeltaY = rawDeltaY;
            this.dragState.velocityX = (event.clientX - this.dragState.lastX) / deltaTime;
            this.dragState.lastX = event.clientX;
            this.dragState.lastTime = now;

            this.applyDragStyles(this.dragState.card, deltaX, deltaY);
            event.preventDefault();
        };

        const finishDrag = (event, shouldCancel = false) => {
            if (!this.dragState || event.pointerId !== this.dragState.pointerId) return;

            const { card: activeCard, deltaX, deltaY, rawDeltaY, velocityX } = this.dragState;
            const strongHorizontalIntent = Math.abs(deltaX) > Math.abs(rawDeltaY) * 0.75;
            const quickFlick = Math.abs(deltaX) > 42 && Math.abs(velocityX) > this.velocityThreshold;
            const shouldSwipe = !shouldCancel && strongHorizontalIntent && (Math.abs(deltaX) > this.minimumSwipeDistance || quickFlick);

            this.dragState = null;
            activeCard.classList.remove('is-dragging');

            if (typeof activeCard.releasePointerCapture === 'function' && activeCard.hasPointerCapture(event.pointerId)) {
                activeCard.releasePointerCapture(event.pointerId);
            }

            if (shouldSwipe) {
                const direction = deltaX >= 0 ? 1 : -1;
                this.completeSwipe(activeCard, direction, deltaY);
            } else {
                this.animateReturn(activeCard);
            }
        };

        const onPointerUp = (event) => finishDrag(event);
        const onPointerCancel = (event) => finishDrag(event, true);

        card.addEventListener('pointerdown', onPointerDown);
        card.addEventListener('pointermove', onPointerMove);
        card.addEventListener('pointerup', onPointerUp);
        card.addEventListener('pointercancel', onPointerCancel);

        this.gestureCleanup = () => {
            card.removeEventListener('pointerdown', onPointerDown);
            card.removeEventListener('pointermove', onPointerMove);
            card.removeEventListener('pointerup', onPointerUp);
            card.removeEventListener('pointercancel', onPointerCancel);
        };
    }

    teardownGestureEvents() {
        if (typeof this.gestureCleanup === 'function') {
            this.gestureCleanup();
        }

        this.gestureCleanup = null;
    }

    applyDragStyles(card, deltaX, deltaY) {
        const width = this.cardStack.offsetWidth || card.offsetWidth || 1;
        const progress = Math.max(-1, Math.min(1, deltaX / width));
        const rotation = progress * this.maxRotation;

        card.style.transform = `translate3d(${deltaX}px, ${deltaY}px, 0) rotate(${rotation}deg)`;
        this.updateNextCard(progress);
        this.updateIndicators(card, progress);
    }

    updateNextCard(progress) {
        const nextCard = this.cards[this.currentCardIndex - 1];
        if (!nextCard) return;

        const absProgress = Math.min(1, Math.abs(progress));
        const opacity = absProgress * 0.96;
        const translateY = 24 - absProgress * 24;
        const scale = 0.98 + absProgress * 0.02;

        nextCard.style.opacity = String(opacity);
        nextCard.style.transform = `translate3d(0, ${translateY}px, 0) scale(${scale})`;
    }

    updateIndicators(card, progress) {
        const likeIndicator = card.querySelector('.like-indicator');
        const nopeIndicator = card.querySelector('.nope-indicator');
        const likeTint = card.querySelector('.swipe-tint--like');
        const nopeTint = card.querySelector('.swipe-tint--nope');

        if (!likeIndicator || !nopeIndicator) return;

        const likeOpacity = Math.max(0, Math.min(1, (progress - 0.06) / 0.34));
        const nopeOpacity = Math.max(0, Math.min(1, (-progress - 0.06) / 0.34));

        likeIndicator.style.opacity = String(likeOpacity);
        likeIndicator.style.transform = `scale(${0.84 + likeOpacity * 0.18}) rotate(5deg)`;

        nopeIndicator.style.opacity = String(nopeOpacity);
        nopeIndicator.style.transform = `scale(${0.84 + nopeOpacity * 0.18}) rotate(-5deg)`;

        if (likeTint) {
            likeTint.style.opacity = String(likeOpacity);
        }

        if (nopeTint) {
            nopeTint.style.opacity = String(nopeOpacity);
        }
    }

    resetIndicators(card) {
        const likeIndicator = card.querySelector('.like-indicator');
        const nopeIndicator = card.querySelector('.nope-indicator');
        const likeTint = card.querySelector('.swipe-tint--like');
        const nopeTint = card.querySelector('.swipe-tint--nope');

        if (likeIndicator) {
            likeIndicator.style.opacity = '0';
            likeIndicator.style.transform = 'scale(0.84) rotate(5deg)';
        }

        if (nopeIndicator) {
            nopeIndicator.style.opacity = '0';
            nopeIndicator.style.transform = 'scale(0.84) rotate(-5deg)';
        }

        if (likeTint) {
            likeTint.style.opacity = '0';
        }

        if (nopeTint) {
            nopeTint.style.opacity = '0';
        }
    }

    animateReturn(card) {
        if (!card) return;

        card.classList.add('is-returning');
        card.style.transition = `transform ${this.enterDuration}ms ease-out, opacity ${this.enterDuration}ms ease-out`;
        card.style.opacity = '1';
        card.style.transform = 'translate3d(0, 0, 0) rotate(0deg)';

        this.updateNextCard(0);
        this.resetIndicators(card);

        window.setTimeout(() => {
            card.classList.remove('is-returning');
        }, this.enterDuration);
    }

    completeSwipe(card, direction, deltaY = 0) {
        if (!card || card.classList.contains('is-swiping') || this.isAnimating) return;

        const foodId = card.dataset.foodId;
        const viewportWidth = Math.max(window.innerWidth, document.documentElement.clientWidth, 1000);
        const offscreenX = direction * (viewportWidth * 1.2);
        const exitRotation = direction * 12;
        const offscreenY = deltaY * 0.2;

        this.isAnimating = true;

        card.classList.add('is-swiping');
        card.style.transition = `transform ${this.exitDuration}ms ease-out, opacity ${this.exitDuration}ms ease-out`;
        this.updateNextCard(1);
        card.style.transform = `translate3d(${offscreenX}px, ${offscreenY}px, 0) rotate(${exitRotation}deg)`;
        card.style.opacity = '0';

        this.updateIndicators(card, direction);
        this.sendSwipe(foodId, direction > 0);

        window.setTimeout(() => {
            card.remove();
            this.updateStack();
        }, this.exitDuration);
    }

    swipeRight(card) {
        this.completeSwipe(card, 1, 0);
    }

    swipeLeft(card) {
        this.completeSwipe(card, -1, 0);
    }

    updateStack() {
        this.cards = Array.from(this.cardStack.querySelectorAll('.swipe-card'));
        this.currentCardIndex = this.cards.length - 1;
        this.isAnimating = false;

        if (this.currentCardIndex >= 0) {
            this.setupCards(true);
        } else {
            this.showNoCardsMessage();
        }
    }

    showNoCardsMessage() {
        const swipeButtons = this.likeBtn?.closest('.swipe-buttons');
        if (swipeButtons) {
            swipeButtons.style.display = 'none';
        }

        this.cardStack.innerHTML = `
            <div class="no-cards-message">
                <div class="message-icon" aria-hidden="true">🍽️</div>
                <h3>Tüm yemekleri gördünüz!</h3>
                <p>Rotayı oluşturmak için aşağıdaki butonu kullanın.</p>
            </div>
        `;

        this.cardStack.style.height = 'auto';
    }

    async sendSwipe(foodId, isLike) {
        const endpoint = isLike ? '/Wizard/Like' : '/Wizard/Dislike';

        try {
            const response = await fetch(endpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ foodItemId: parseInt(foodId, 10) })
            });

            const data = await response.json();

            if (data.success && isLike && this.counterValue) {
                this.counterValue.textContent = data.likedCount;
                this.animateCounter();
            }
        } catch (error) {
            console.error('Swipe error:', error);
        }
    }

    animateCounter() {
        const counter = document.getElementById('liked-counter');

        if (!counter) return;

        counter.style.transform = 'scale(1.05)';

        window.setTimeout(() => {
            counter.style.transform = 'scale(1)';
        }, 180);
    }

    bindButtons() {
        if (this.likeBtn) {
            this.likeBtn.addEventListener('click', () => {
                const card = this.topCard;
                if (card && !this.isAnimating) {
                    this.swipeRight(card);
                }
            });
        }

        if (this.nopeBtn) {
            this.nopeBtn.addEventListener('click', () => {
                const card = this.topCard;
                if (card && !this.isAnimating) {
                    this.swipeLeft(card);
                }
            });
        }
    }

    bindKeyboard() {
        document.addEventListener('keydown', (event) => {
            const card = this.topCard;

            if (!card || this.isAnimating) return;

            if (event.key === 'ArrowRight') {
                this.swipeRight(card);
            } else if (event.key === 'ArrowLeft') {
                this.swipeLeft(card);
            }
        });
    }
}

SwipeCards.prototype.preventNativeDrag = (event) => {
    event.preventDefault();
};

document.addEventListener('DOMContentLoaded', () => {
    new SwipeCards();
});

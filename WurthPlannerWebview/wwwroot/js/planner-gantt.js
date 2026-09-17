// Gestion du drag (déplacement) et du resize (poignée de fin) des barres
// du planning. On reste en JS pur pour la fluidité du pointeur (pas de
// round-trip Blazor à chaque pixel) et on ne notifie .NET qu'à la fin du
// geste (pointerup), avec les deltas calculés.

window.plannerGantt = {
    _state: null,

    /// dotNetRef: référence vers le composant Blazor (pour invoke).
    /// dayWidth: largeur en pixels d'une journée dans la grille.
    /// rowHeight: hauteur en pixels d'une rangée de swimlane.
    init: function (dotNetRef, dayWidth, rowHeight) {
        this._dotNetRef = dotNetRef;
        this._dayWidth = dayWidth;
        this._rowHeight = rowHeight;

        window.addEventListener('pointermove', this._onPointerMove.bind(this));
        window.addEventListener('pointerup', this._onPointerUp.bind(this));
    },

    updateMetrics: function (dayWidth, rowHeight) {
        this._dayWidth = dayWidth;
        this._rowHeight = rowHeight;
    },

    /// Démarre un déplacement (move) de barre. workItemId est la clé utilisée
    /// pour notifier .NET à la fin du geste.
    startMove: function (clientX, clientY, workItemId, rowIndex) {
        this._beginDrag(clientX, clientY, workItemId, 'move', rowIndex);
    },

    /// Démarre un redimensionnement (poignée de fin uniquement).
    startResize: function (clientX, clientY, workItemId, rowIndex) {
        this._beginDrag(clientX, clientY, workItemId, 'resize', rowIndex);
    },

    _beginDrag: function (clientX, clientY, workItemId, mode, rowIndex) {
        this._state = {
            workItemId: workItemId,
            mode: mode,
            startX: clientX,
            startY: clientY,
            startRowIndex: rowIndex,
            currentRowIndex: rowIndex,
            dayDelta: 0,
            durationDayDelta: 0,
            moved: false
        };

        const el = document.querySelector(`[data-workitem-id="${workItemId}"]`);
        if (el) el.classList.add('planner-bar-dragging');
    },

    _onPointerMove: function (event) {
        if (!this._state) return;

        const dx = event.clientX - this._state.startX;
        const dy = event.clientY - this._state.startY;

        if (Math.abs(dx) > 3 || Math.abs(dy) > 3) {
            this._state.moved = true;
        }

        const dayDelta = Math.round(dx / this._dayWidth);
        const laneDelta = Math.round(dy / this._rowHeight);

        if (this._state.mode === 'move') {
            this._state.dayDelta = dayDelta;
            this._state.currentRowIndex = this._state.startRowIndex + laneDelta;
            this._applyMovePreview();
        } else {
            // resize: on ne modifie que la durée (largeur), jamais la ligne.
            this._state.durationDayDelta = dayDelta;
            this._applyResizePreview();
        }
    },

    _applyMovePreview: function () {
        const el = document.querySelector(`[data-workitem-id="${this._state.workItemId}"]`);
        if (!el) return;
        el.style.transform = `translate(${this._state.dayDelta * this._dayWidth}px, ${(this._state.currentRowIndex - this._state.startRowIndex) * this._rowHeight}px)`;
    },

    _applyResizePreview: function () {
        const el = document.querySelector(`[data-workitem-id="${this._state.workItemId}"]`);
        if (!el) return;
        const baseWidth = parseFloat(el.dataset.baseWidth || '0');
        const newWidth = Math.max(this._dayWidth, baseWidth + this._state.durationDayDelta * this._dayWidth);
        el.style.width = `${newWidth}px`;
    },

    _onPointerUp: function (event) {
        if (!this._state) return;

        const state = this._state;
        const el = document.querySelector(`[data-workitem-id="${state.workItemId}"]`);
        if (el) {
            el.classList.remove('planner-bar-dragging');
            el.style.transform = '';
            //el.style.width = '';
        }
        this._state = null;

        if (!state.moved) return; // simple clic, pas un drag

        if (state.mode === 'move') {
            if (state.dayDelta !== 0 || state.currentRowIndex !== state.startRowIndex) {
                this._dotNetRef.invokeMethodAsync('OnBarMoved', state.workItemId, state.dayDelta, state.currentRowIndex);
            }
        } else {
            if (state.durationDayDelta !== 0) {
                this._dotNetRef.invokeMethodAsync('OnBarResized', state.workItemId, state.durationDayDelta);
            }
        }
    }
};

/* BoredProgrammer — UI behaviours: theme, reveal, toasts, AI chat, AI summary */
(function () {
    "use strict";

    /* ---------------- theme ---------------- */
    const root = document.documentElement;
    const themeToggle = document.getElementById("themeToggle");
    const savedTheme = (() => { try { return localStorage.getItem("bp-theme"); } catch (e) { return null; } })();
    if (savedTheme === "light") root.setAttribute("data-bs-theme", "light");

    themeToggle?.addEventListener("click", () => {
        const next = root.getAttribute("data-bs-theme") === "light" ? "dark" : "light";
        root.setAttribute("data-bs-theme", next);
        try { localStorage.setItem("bp-theme", next); } catch (e) { }
    });

    /* ---------------- toasts ---------------- */
    window.bpToast = function (message, type) {
        const host = document.getElementById("toastHost");
        if (!host) return;
        const el = document.createElement("div");
        el.className = "bp-toast" + (type === "error" ? " error" : "");
        el.innerHTML = `<i class="bi ${type === "error" ? "bi-exclamation-circle" : "bi-check-circle"}"></i><span></span>`;
        el.querySelector("span").textContent = message;
        host.appendChild(el);
        setTimeout(() => {
            el.style.opacity = "0";
            el.style.transform = "translateY(8px)";
            setTimeout(() => el.remove(), 250);
        }, 3200);
    };

    /* ---------------- reveal on scroll ---------------- */
    const revealEls = document.querySelectorAll(".reveal");
    if ("IntersectionObserver" in window && revealEls.length) {
        const io = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add("in");
                    io.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12 });
        revealEls.forEach((el) => io.observe(el));
    } else {
        revealEls.forEach((el) => el.classList.add("in"));
    }

    /* ---------------- counters ---------------- */
    document.querySelectorAll("[data-count]").forEach((el) => {
        const target = parseInt(el.getAttribute("data-count"), 10);
        if (Number.isNaN(target)) return;
        let current = 0;
        const step = Math.max(1, Math.ceil(target / 32));
        const timer = setInterval(() => {
            current += step;
            if (current >= target) { current = target; clearInterval(timer); }
            el.textContent = current;
        }, 34);
    });

    /* ---------------- copy buttons ---------------- */
    document.querySelectorAll("[data-copy]").forEach((btn) => {
        btn.addEventListener("click", async () => {
            const value = btn.getAttribute("data-copy") || window.location.href;
            try {
                await navigator.clipboard.writeText(value);
                window.bpToast("Link copied to clipboard");
            } catch (e) {
                window.bpToast("Could not copy the link", "error");
            }
        });
    });

    /* ---------------- tiny markdown renderer ---------------- */
    function escapeHtml(value) {
        return value.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
    }

    function renderMarkdown(text) {
        const safe = escapeHtml(text || "");
        const lines = safe.split(/\r?\n/);
        let html = "";
        let inList = false;

        const inline = (value) => value
            .replace(/\*\*(.+?)\*\*/g, "<strong>$1</strong>")
            .replace(/`([^`]+)`/g, "<code>$1</code>")
            .replace(/\[([^\]]+)\]\((\/[^\s)]*|https?:\/\/[^\s)]+)\)/g, '<a href="$2" target="_blank" rel="noopener">$1</a>');

        lines.forEach((line) => {
            const trimmed = line.trim();
            if (/^([-*]|\d+\.)\s+/.test(trimmed)) {
                if (!inList) { html += "<ul>"; inList = true; }
                html += `<li>${inline(trimmed.replace(/^([-*]|\d+\.)\s+/, ""))}</li>`;
                return;
            }
            if (inList) { html += "</ul>"; inList = false; }
            if (trimmed.length) html += `<p>${inline(trimmed)}</p>`;
        });

        if (inList) html += "</ul>";
        return html || "<p></p>";
    }

    /* ---------------- AI chat ---------------- */
    function initAiChat(rootEl) {
        const endpoint = rootEl.getAttribute("data-endpoint");
        const messagesEl = rootEl.querySelector("[data-ai-messages]");
        const form = rootEl.querySelector("[data-ai-form]");
        const input = rootEl.querySelector("[data-ai-input]");
        const sendBtn = rootEl.querySelector("[data-ai-send]");
        if (!endpoint || !messagesEl || !form || !input) return;

        const history = [];
        let busy = false;

        function scrollBottom() {
            requestAnimationFrame(() => { messagesEl.scrollTop = messagesEl.scrollHeight; });
        }

        function append(role, content) {
            const el = document.createElement("div");
            el.className = "ai-msg " + (role === "user" ? "ai-msg-user" : "ai-msg-bot");
            el.innerHTML = role === "user" ? `<p>${escapeHtml(content)}</p>` : renderMarkdown(content);
            messagesEl.appendChild(el);
            scrollBottom();
            return el;
        }

        function showTyping() {
            const el = document.createElement("div");
            el.className = "ai-msg ai-msg-bot";
            el.innerHTML = '<span class="typing"><span></span><span></span><span></span></span>';
            messagesEl.appendChild(el);
            scrollBottom();
            return el;
        }

        async function ask(question) {
            if (busy) return;
            const message = (question || "").trim();
            if (!message) return;

            busy = true;
            input.value = "";
            input.style.height = "auto";
            if (sendBtn) sendBtn.disabled = true;

            append("user", message);
            const typing = showTyping();

            try {
                const response = await fetch(endpoint, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ message, history: history.slice(-8) })
                });

                if (response.status === 429) {
                    typing.remove();
                    append("assistant", "You're sending messages a little too fast — please wait a minute and try again.");
                    return;
                }

                const data = await response.json();
                typing.remove();
                const reply = data.reply || "I couldn't answer that one. Try rephrasing?";
                append("assistant", reply);

                history.push({ role: "user", content: message });
                history.push({ role: "assistant", content: reply });
                if (history.length > 16) history.splice(0, history.length - 16);
            } catch (error) {
                typing.remove();
                append("assistant", "The AI service is unreachable right now. Please try again in a moment.");
            } finally {
                busy = false;
                if (sendBtn) sendBtn.disabled = false;
                input.focus();
            }
        }

        form.addEventListener("submit", (event) => {
            event.preventDefault();
            ask(input.value);
        });

        input.addEventListener("keydown", (event) => {
            if (event.key === "Enter" && !event.shiftKey) {
                event.preventDefault();
                ask(input.value);
            }
        });

        input.addEventListener("input", () => {
            input.style.height = "auto";
            input.style.height = Math.min(input.scrollHeight, 120) + "px";
        });

        rootEl.querySelectorAll("[data-prompt]").forEach((button) => {
            button.addEventListener("click", () => ask(button.getAttribute("data-prompt")));
        });

        const openBtn = document.querySelector("[data-ai-open]");
        const closeBtn = rootEl.querySelector("[data-ai-close]");
        const panel = rootEl.querySelector("[data-ai-panel]");
        if (openBtn && panel) {
            openBtn.addEventListener("click", () => {
                panel.classList.add("open");
                panel.setAttribute("aria-hidden", "false");
                openBtn.setAttribute("aria-expanded", "true");
                setTimeout(() => input.focus(), 220);
            });
        }
        closeBtn?.addEventListener("click", () => {
            panel?.classList.remove("open");
            panel?.setAttribute("aria-hidden", "true");
            openBtn?.setAttribute("aria-expanded", "false");
        });
        document.addEventListener("keydown", (event) => {
            if (event.key === "Escape" && panel?.classList.contains("open")) {
                panel.classList.remove("open");
                panel.setAttribute("aria-hidden", "true");
                openBtn?.setAttribute("aria-expanded", "false");
            }
        });

        rootEl._aiAsk = ask;
    }

    const chatRoots = document.querySelectorAll("[data-ai-chat]");
    chatRoots.forEach(initAiChat);

    /* ---------------- AI summary ---------------- */
    document.querySelectorAll("[data-ai-summary]").forEach((button) => {
        button.addEventListener("click", async () => {
            const targetId = button.getAttribute("data-target");
            const target = targetId ? document.getElementById(targetId) : null;
            if (!target) return;

            const original = button.innerHTML;
            button.disabled = true;
            button.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Summarising…';

            try {
                const response = await fetch(button.getAttribute("data-endpoint"), {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ postId: parseInt(button.getAttribute("data-post-id"), 10) })
                });
                const data = await response.json();
                target.innerHTML = `<div class="ai-summary-head"><i class="bi bi-stars"></i><strong>AI summary</strong></div><div class="summary-body">${renderMarkdown(data.summary || "No summary available.")}</div>`;
                target.classList.remove("d-none");
            } catch (error) {
                target.innerHTML = '<div class="ai-summary-head"><i class="bi bi-exclamation-circle"></i><strong>AI summary</strong></div><div class="summary-body">The AI service is unreachable right now.</div>';
                target.classList.remove("d-none");
            } finally {
                button.disabled = false;
                button.innerHTML = original;
            }
        });
    });

    /* ---------------- console signature ---------------- */
    console.log("%canascloud", "font-weight:700;font-size:16px;color:#c7dbff", "— .NET + AI live demo");
})();

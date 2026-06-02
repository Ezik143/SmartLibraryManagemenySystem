const STORAGE_TOKEN = "smartlib_token";
const STORAGE_USER = "smartlib_user";
const SIGNUP_EMAIL = "smartlib_signup_email";

const state = {
    token: localStorage.getItem(STORAGE_TOKEN) || "",
    user: readStoredUser(),
    records: [],
    editingId: null,
    currentView: "all"
};

function readStoredUser() {
    try {
        return JSON.parse(localStorage.getItem(STORAGE_USER) || "null");
    } catch {
        return null;
    }
}

function getValue(record, key) {
    if (!record) {
        return undefined;
    }

    const pascalKey = key.charAt(0).toUpperCase() + key.slice(1);
    return record[key] ?? record[pascalKey];
}

function isSignedIn() {
    return Boolean(state.token && state.user);
}

function isAdminUser() {
    return getValue(state.user, "isAdmin") === true || getValue(state.user, "role") === "Admin";
}

function setStatus(element, text, isError = false) {
    element.textContent = text;
    element.classList.toggle("error", isError);
}

function setButtonLoading(button, isLoading, text) {
    if (!button) {
        return;
    }

    if (!button.dataset.defaultText) {
        button.dataset.defaultText = button.textContent;
    }

    button.disabled = isLoading;
    button.textContent = isLoading ? text : button.dataset.defaultText;
}

function normalizeDate(value) {
    if (!value) {
        return "-";
    }

    return String(value).slice(0, 10);
}

async function apiRequest(path, options = {}) {
    const { auth = true, ...fetchOptions } = options;
    const headers = {
        ...(fetchOptions.body ? { "Content-Type": "application/json" } : {}),
        ...(auth && state.token ? { Authorization: `Bearer ${state.token}` } : {}),
        ...(fetchOptions.headers || {})
    };

    const response = await fetch(path, {
        ...fetchOptions,
        headers
    });

    if (response.status === 204) {
        return null;
    }

    const contentType = response.headers.get("content-type") || "";
    const payload = contentType.includes("application/json")
        ? await response.json()
        : await response.text();

    if (!response.ok) {
        const message = payload?.detail || payload?.title || payload?.message || payload || response.statusText;
        throw new Error(message);
    }

    return payload;
}

function persistSession(user) {
    const token = getValue(user, "token");
    if (!token) {
        throw new Error("Login response did not include a token.");
    }

    state.token = token;
    state.user = user;
    localStorage.setItem(STORAGE_TOKEN, token);
    localStorage.setItem(STORAGE_USER, JSON.stringify(user));
}

function clearSession() {
    state.token = "";
    state.user = null;
    localStorage.removeItem(STORAGE_TOKEN);
    localStorage.removeItem(STORAGE_USER);
}

function initLoginPage() {
    if (isSignedIn()) {
        window.location.replace("index.html");
        return;
    }

    const form = document.getElementById("login-form");
    const email = document.getElementById("email");
    const password = document.getElementById("password");
    const button = document.getElementById("login-button");
    const message = document.getElementById("auth-message");
    const signupEmail = sessionStorage.getItem(SIGNUP_EMAIL);

    if (signupEmail) {
        email.value = signupEmail;
        sessionStorage.removeItem(SIGNUP_EMAIL);
        setStatus(message, "Account created. Sign in to continue.");
    }

    form.addEventListener("submit", async event => {
        event.preventDefault();
        setStatus(message, "Signing in...");
        setButtonLoading(button, true, "Signing In");

        try {
            const user = await apiRequest("/api/Auth/login", {
                method: "POST",
                auth: false,
                body: JSON.stringify({
                    email: email.value.trim(),
                    password: password.value
                })
            });

            persistSession(user);
            window.location.href = "index.html";
        } catch (error) {
            setStatus(message, error.message, true);
        } finally {
            setButtonLoading(button, false);
        }
    });
}

function initSignupPage() {
    if (isSignedIn()) {
        window.location.replace("index.html");
        return;
    }

    const form = document.getElementById("signup-form");
    const role = document.getElementById("signup-role");
    const name = document.getElementById("signup-name");
    const email = document.getElementById("signup-email");
    const password = document.getElementById("signup-password");
    const department = document.getElementById("signup-department");
    const section = document.getElementById("signup-section");
    const yearLevel = document.getElementById("signup-year-level");
    const departmentId = document.getElementById("signup-department-id");
    const studentFields = document.getElementById("student-fields");
    const teacherFields = document.getElementById("teacher-fields");
    const button = document.getElementById("signup-button");
    const message = document.getElementById("auth-message");

    function updateSignupFields() {
        const isTeacher = role.value === "Teacher";
        teacherFields.classList.toggle("hidden", !isTeacher);
        studentFields.classList.toggle("hidden", isTeacher);
        departmentId.required = isTeacher;
        section.required = !isTeacher;
        yearLevel.required = !isTeacher;
    }

    role.addEventListener("change", updateSignupFields);
    updateSignupFields();

    form.addEventListener("submit", async event => {
        event.preventDefault();
        setStatus(message, "Creating account...");
        setButtonLoading(button, true, "Creating");

        const payload = {
            name: name.value.trim(),
            email: email.value.trim(),
            password: password.value,
            department: department.value,
            isActive: true
        };

        let path = "/api/Auth/register/student";
        if (role.value === "Teacher") {
            path = "/api/Auth/register/teacher";
            payload.departmentId = Number(departmentId.value);
        } else {
            payload.section = section.value.trim();
            payload.yearLevel = yearLevel.value.trim();
            payload.createdAt = new Date().toISOString();
        }

        try {
            await apiRequest(path, {
                method: "POST",
                auth: false,
                body: JSON.stringify(payload)
            });

            sessionStorage.setItem(SIGNUP_EMAIL, payload.email);
            window.location.href = "login.html";
        } catch (error) {
            setStatus(message, error.message, true);
        } finally {
            setButtonLoading(button, false);
        }
    });
}

function initDashboardPage() {
    if (!isSignedIn()) {
        window.location.replace("login.html");
        return;
    }

    const elements = {
        logoutButton: document.getElementById("logout-button"),
        sessionName: document.getElementById("session-name"),
        message: document.getElementById("message"),
        recordForm: document.getElementById("record-form"),
        newRecordLink: document.getElementById("new-record-link"),
        formTitle: document.getElementById("form-title"),
        editingLabel: document.getElementById("editing-label"),
        cancelEditButton: document.getElementById("cancel-edit-button"),
        userId: document.getElementById("user-id"),
        bookId: document.getElementById("book-id"),
        borrowDate: document.getElementById("borrow-date"),
        dueDate: document.getElementById("due-date"),
        returnDate: document.getElementById("return-date"),
        recordStatus: document.getElementById("record-status"),
        recordsBody: document.getElementById("records-body"),
        recordCount: document.getElementById("record-count"),
        searchInput: document.getElementById("search-input"),
        statusFilter: document.getElementById("status-filter"),
        loadAllButton: document.getElementById("load-all-button"),
        loadMineButton: document.getElementById("load-mine-button"),
        loadOverdueButton: document.getElementById("load-overdue-button"),
        loadReturnedButton: document.getElementById("load-returned-button"),
        statTotal: document.getElementById("stat-total"),
        statBorrowed: document.getElementById("stat-borrowed"),
        statOverdue: document.getElementById("stat-overdue"),
        statReturned: document.getElementById("stat-returned")
    };

    function setDashboardMessage(text, isError = false) {
        setStatus(elements.message, text, isError);
    }

    function renderSession() {
        const displayName = getValue(state.user, "name") || getValue(state.user, "email") || "Signed in";
        const role = getValue(state.user, "role") || "User";
        elements.sessionName.textContent = `${displayName} (${role})`;
    }

    function renderPermissions() {
        const isAdmin = isAdminUser();
        elements.recordForm.classList.toggle("hidden", !isAdmin);
        elements.newRecordLink.classList.toggle("hidden", !isAdmin);
        elements.loadAllButton.disabled = !isAdmin;
        elements.loadOverdueButton.disabled = !isAdmin;
        elements.loadReturnedButton.disabled = !isAdmin;
    }

    async function loadRecords(view = state.currentView) {
        state.currentView = view;
        setDashboardMessage("Loading records...");

        try {
            let path = "/api/BorrowRecord";

            if (view === "mine") {
                const currentUserId = getValue(state.user, "userId");
                if (!currentUserId) {
                    throw new Error("Current user ID is missing.");
                }
                path = `/api/BorrowRecord/user/${encodeURIComponent(currentUserId)}`;
            }

            if (view === "overdue") {
                path = "/api/BorrowRecord/Overdue";
            }

            if (view === "returned") {
                path = "/api/BorrowRecord/Paid";
            }

            const records = await apiRequest(path);
            state.records = Array.isArray(records) ? records : [];
            renderRecords();
            setDashboardMessage(`${state.records.length} record(s) loaded.`);
        } catch (error) {
            setDashboardMessage(error.message, true);
        }
    }

    function getFormPayload() {
        return {
            userId: elements.userId.value.trim(),
            bookId: Number(elements.bookId.value),
            borrowDate: elements.borrowDate.value,
            dueDate: elements.dueDate.value,
            returnDate: elements.returnDate.value || null,
            status: elements.recordStatus.value
        };
    }

    async function saveRecord(event) {
        event.preventDefault();

        const payload = getFormPayload();
        const isEditing = state.editingId !== null;
        const path = isEditing ? `/api/BorrowRecord/${state.editingId}` : "/api/BorrowRecord";

        setDashboardMessage(isEditing ? "Updating record..." : "Creating record...");

        try {
            await apiRequest(path, {
                method: isEditing ? "PUT" : "POST",
                body: JSON.stringify(payload)
            });

            resetForm();
            await loadRecords(state.currentView);
            setDashboardMessage(isEditing ? "Record updated." : "Record created.");
        } catch (error) {
            setDashboardMessage(error.message, true);
        }
    }

    function editRecord(recordId) {
        if (!isAdminUser()) {
            setDashboardMessage("Only admin accounts can edit borrow records.", true);
            return;
        }

        const record = state.records.find(item => String(getValue(item, "borrowRecordId")) === String(recordId));
        if (!record) {
            return;
        }

        state.editingId = recordId;
        elements.formTitle.textContent = "Update Borrow Record";
        elements.editingLabel.textContent = `#${recordId}`;
        elements.userId.value = getValue(record, "userId") || "";
        elements.bookId.value = getValue(record, "bookId") || "";
        elements.borrowDate.value = normalizeDate(getValue(record, "borrowDate")) === "-" ? "" : normalizeDate(getValue(record, "borrowDate"));
        elements.dueDate.value = normalizeDate(getValue(record, "dueDate")) === "-" ? "" : normalizeDate(getValue(record, "dueDate"));
        elements.returnDate.value = normalizeDate(getValue(record, "returnDate")) === "-" ? "" : normalizeDate(getValue(record, "returnDate"));
        elements.recordStatus.value = getValue(record, "status") || "BORROWED";
        elements.recordForm.scrollIntoView({ behavior: "smooth", block: "start" });
    }

    function resetForm() {
        state.editingId = null;
        elements.recordForm.reset();
        elements.formTitle.textContent = "Create Borrow Record";
        elements.editingLabel.textContent = "New";
    }

    async function deleteRecord(recordId) {
        if (!isAdminUser()) {
            setDashboardMessage("Only admin accounts can delete borrow records.", true);
            return;
        }

        if (!confirm(`Delete borrow record #${recordId}?`)) {
            return;
        }

        setDashboardMessage("Deleting record...");

        try {
            await apiRequest(`/api/BorrowRecord/${recordId}`, { method: "DELETE" });
            await loadRecords(state.currentView);
            setDashboardMessage("Record deleted.");
        } catch (error) {
            setDashboardMessage(error.message, true);
        }
    }

    async function returnRecord(recordId) {
        if (!isAdminUser()) {
            setDashboardMessage("Only admin accounts can return books from this dashboard.", true);
            return;
        }

        setDashboardMessage("Returning book...");

        try {
            await apiRequest(`/api/BorrowRecord/return/${recordId}`, { method: "POST" });
            await loadRecords(state.currentView);
            setDashboardMessage("Book returned.");
        } catch (error) {
            setDashboardMessage(error.message, true);
        }
    }

    function getFilteredRecords() {
        const query = elements.searchInput.value.trim().toLowerCase();
        const status = elements.statusFilter.value;

        return state.records.filter(record => {
            const recordStatus = String(getValue(record, "status") || "");
            const matchesStatus = !status || recordStatus === status;
            const haystack = [
                getValue(record, "borrowRecordId"),
                getValue(record, "userId"),
                getValue(record, "bookId"),
                recordStatus
            ].join(" ").toLowerCase();

            return matchesStatus && (!query || haystack.includes(query));
        });
    }

    function renderRecords() {
        const records = getFilteredRecords();
        elements.recordCount.textContent = `${records.length} shown`;
        updateStats(state.records);

        if (records.length === 0) {
            elements.recordsBody.innerHTML = '<tr><td colspan="8" class="empty-cell">No records found.</td></tr>';
            return;
        }

        elements.recordsBody.innerHTML = records.map(record => {
            const id = getValue(record, "borrowRecordId");
            const status = String(getValue(record, "status") || "BORROWED");
            const statusClass = status.toLowerCase();
            const adminActions = isAdminUser()
                ? `
                    <button class="table-button" type="button" data-action="edit" data-id="${id}">Edit</button>
                    <button class="table-button" type="button" data-action="return" data-id="${id}">Return</button>
                    <button class="table-button" type="button" data-action="delete" data-id="${id}">Delete</button>
                `
                : '<span class="muted-text">View only</span>';

            return `
                <tr>
                    <td>#${id}</td>
                    <td>${escapeHtml(getValue(record, "userId") || "")}</td>
                    <td>${escapeHtml(getValue(record, "bookId") || "")}</td>
                    <td>${normalizeDate(getValue(record, "borrowDate"))}</td>
                    <td>${normalizeDate(getValue(record, "dueDate"))}</td>
                    <td>${normalizeDate(getValue(record, "returnDate"))}</td>
                    <td><span class="status ${statusClass}">${escapeHtml(status)}</span></td>
                    <td>
                        <div class="row-actions">
                            ${adminActions}
                        </div>
                    </td>
                </tr>
            `;
        }).join("");
    }

    function updateStats(records) {
        elements.statTotal.textContent = records.length;
        elements.statBorrowed.textContent = records.filter(item => getValue(item, "status") === "BORROWED").length;
        elements.statOverdue.textContent = records.filter(item => getValue(item, "status") === "OVERDUE").length;
        elements.statReturned.textContent = records.filter(item => getValue(item, "status") === "RETURNED").length;
    }

    function escapeHtml(value) {
        return String(value)
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;")
            .replaceAll("'", "&#039;");
    }

    elements.logoutButton.addEventListener("click", () => {
        clearSession();
        window.location.href = "login.html";
    });
    elements.recordForm.addEventListener("submit", saveRecord);
    elements.cancelEditButton.addEventListener("click", resetForm);
    elements.searchInput.addEventListener("input", renderRecords);
    elements.statusFilter.addEventListener("change", renderRecords);
    elements.loadAllButton.addEventListener("click", () => loadRecords("all"));
    elements.loadMineButton.addEventListener("click", () => loadRecords("mine"));
    elements.loadOverdueButton.addEventListener("click", () => loadRecords("overdue"));
    elements.loadReturnedButton.addEventListener("click", () => loadRecords("returned"));

    elements.recordsBody.addEventListener("click", event => {
        const button = event.target.closest("button[data-action]");
        if (!button) {
            return;
        }

        const action = button.dataset.action;
        const id = button.dataset.id;

        if (action === "edit") {
            editRecord(id);
        }

        if (action === "return") {
            returnRecord(id);
        }

        if (action === "delete") {
            deleteRecord(id);
        }
    });

    renderSession();
    renderPermissions();
    renderRecords();
    loadRecords(isAdminUser() ? "all" : "mine");
}

const page = document.body.dataset.page;

if (page === "login") {
    initLoginPage();
}

if (page === "signup") {
    initSignupPage();
}

if (page === "dashboard") {
    initDashboardPage();
}

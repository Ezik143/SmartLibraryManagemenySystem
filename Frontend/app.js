const API_BASE_URL = "";
const STORAGE_TOKEN = "smartlib_token";
const STORAGE_USER = "smartlib_user";
const SIGNUP_EMAIL = "smartlib_signup_email";

const state = {
    token: localStorage.getItem(STORAGE_TOKEN) || "",
    user: readStoredUser(),
    records: [],
    editingId: null,
    currentView: "all",
    books: [],
    bookEditingId: null,
    students: [],
    fines: []
};

function readStoredUser() {
    try {
        return JSON.parse(localStorage.getItem(STORAGE_USER) || "null");
    } catch {
        return null;
    }
}

function getValue(record, key) {
    if (!record) return undefined;
    const pascalKey = key.charAt(0).toUpperCase() + key.slice(1);
    return record[key] ?? record[pascalKey];
}

function isSignedIn() {
    return Boolean(state.token && state.user);
}

function isAdminUser() {
    return getValue(state.user, "isAdmin") === true || getValue(state.user, "role") === "Admin";
}

function getUserId() {
    return getValue(state.user, "userId") || getValue(state.user, "id") || "";
}

function setStatus(element, text, isError = false) {
    element.textContent = text;
    element.classList.toggle("error", isError);
}

function setButtonLoading(button, isLoading, text) {
    if (!button) return;
    if (!button.dataset.defaultText) {
        button.dataset.defaultText = button.textContent;
    }
    button.disabled = isLoading;
    button.textContent = isLoading ? text : button.dataset.defaultText;
}

function normalizeDate(value) {
    if (!value) return "-";
    return String(value).slice(0, 10);
}

async function apiRequest(path, options = {}) {
    const { auth = true, ...fetchOptions } = options;
    const url = API_BASE_URL + path;
    const headers = {
        ...(fetchOptions.body ? { "Content-Type": "application/json" } : {}),
        ...(auth && state.token ? { Authorization: `Bearer ${state.token}` } : {}),
        ...(fetchOptions.headers || {})
    };
    const response = await fetch(url, { ...fetchOptions, headers });
    if (response.status === 204) return null;
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
    if (!token) throw new Error("Login response did not include a token.");
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

// ---------- NAVIGATION ----------
function initNavigation() {
    const navLinks = document.querySelectorAll(".nav-link[data-section]");
    navLinks.forEach(link => {
        link.addEventListener("click", event => {
            event.preventDefault();
            const section = link.dataset.section;
            navLinks.forEach(l => l.classList.remove("active"));
            link.classList.add("active");
            document.querySelectorAll(".dashboard-section").forEach(s => s.classList.add("hidden"));
            const target = document.getElementById("section-" + section);
            if (target) target.classList.remove("hidden");
        });
    });

    // Show Students nav link only for admins
    document.querySelectorAll(".nav-link.admin-only").forEach(el => {
        el.classList.toggle("hidden", !isAdminUser());
    });
}

var E = { amp: 38, lt: 60, gt: 62, quot: 34, x27: 39 };

function escapeHtml(value) {
    return String(value).replace(/[&<>"']/g, function (c) {
        if (c === "&") return "&" + String.fromCharCode(E.amp) + "amp;";
        if (c === "<") return "&" + String.fromCharCode(E.lt) + ";";
        if (c === ">") return "&" + String.fromCharCode(E.gt) + ";";
        if (c === '"') return "&" + String.fromCharCode(E.quot) + ";";
        return "&#" + String.fromCharCode(E.x27) + ";";
    });
}

// =====================================================================
//  LOGIN
// =====================================================================
function initLoginPage() {
    if (isSignedIn()) { window.location.replace("index.html"); return; }
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
                method: "POST", auth: false,
                body: JSON.stringify({ email: email.value.trim(), password: password.value })
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

// =====================================================================
//  SIGNUP
// =====================================================================
function initSignupPage() {
    if (isSignedIn()) { window.location.replace("index.html"); return; }
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
            await apiRequest(path, { method: "POST", auth: false, body: JSON.stringify(payload) });
            sessionStorage.setItem(SIGNUP_EMAIL, payload.email);
            window.location.href = "login.html";
        } catch (error) {
            setStatus(message, error.message, true);
        } finally {
            setButtonLoading(button, false);
        }
    });
}

// =====================================================================
//  DASHBOARD - BORROW RECORDS
// =====================================================================
function initDashboardPage() {
    if (!isSignedIn()) { window.location.replace("login.html"); return; }

    initNavigation();

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

    function setMsg(text, isError = false) { setStatus(elements.message, text, isError); }

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

    async function loadRecords(view) {
        if (view) state.currentView = view;
        setMsg("Loading records...");
        try {
            let path = "/api/BorrowRecord";
            if (state.currentView === "mine") {
                const uid = getUserId();
                if (!uid) throw new Error("Current user ID is missing.");
                path = `/api/BorrowRecord/user/${encodeURIComponent(uid)}`;
            }
            if (state.currentView === "overdue") path = "/api/BorrowRecord/Overdue";
            if (state.currentView === "returned") path = "/api/BorrowRecord/Paid";
            const records = await apiRequest(path);
            state.records = Array.isArray(records) ? records : [];
            renderRecords();
            setMsg(`${state.records.length} record(s) loaded.`);
        } catch (error) {
            setMsg(error.message, true);
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
        setMsg(isEditing ? "Updating record..." : "Creating record...");
        try {
            await apiRequest(path, { method: isEditing ? "PUT" : "POST", body: JSON.stringify(payload) });
            resetForm();
            await loadRecords(state.currentView);
            setMsg(isEditing ? "Record updated." : "Record created.");
        } catch (error) {
            setMsg(error.message, true);
        }
    }

    function editRecord(recordId) {
        if (!isAdminUser()) { setMsg("Only admin accounts can edit borrow records.", true); return; }
        const record = state.records.find(item => String(getValue(item, "borrowRecordId")) === String(recordId));
        if (!record) return;
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
        if (!isAdminUser()) { setMsg("Only admin accounts can delete borrow records.", true); return; }
        if (!confirm(`Delete borrow record #${recordId}?`)) return;
        setMsg("Deleting record...");
        try {
            await apiRequest(`/api/BorrowRecord/${recordId}`, { method: "DELETE" });
            await loadRecords(state.currentView);
            setMsg("Record deleted.");
        } catch (error) {
            setMsg(error.message, true);
        }
    }

    async function returnRecord(recordId) {
        if (!isAdminUser()) { setMsg("Only admin accounts can return books from this dashboard.", true); return; }
        setMsg("Returning book...");
        try {
            await apiRequest(`/api/BorrowRecord/return/${recordId}`, { method: "POST" });
            await loadRecords(state.currentView);
            setMsg("Book returned.");
        } catch (error) {
            setMsg(error.message, true);
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
                ? `<button class="table-button" type="button" data-action="edit" data-id="${id}">Edit</button>
                   <button class="table-button" type="button" data-action="return" data-id="${id}">Return</button>
                   <button class="table-button" type="button" data-action="delete" data-id="${id}">Delete</button>`
                : '<span class="muted-text">View only</span>';
            return `<tr>
                <td>#${id}</td>
                <td>${escapeHtml(getValue(record, "userId") || "")}</td>
                <td>${escapeHtml(getValue(record, "bookId") || "")}</td>
                <td>${normalizeDate(getValue(record, "borrowDate"))}</td>
                <td>${normalizeDate(getValue(record, "dueDate"))}</td>
                <td>${normalizeDate(getValue(record, "returnDate"))}</td>
                <td><span class="status ${statusClass}">${escapeHtml(status)}</span></td>
                <td><div class="row-actions">${adminActions}</div></td>
            </tr>`;
        }).join("");
    }

    function updateStats(records) {
        elements.statTotal.textContent = records.length;
        elements.statBorrowed.textContent = records.filter(item => getValue(item, "status") === "BORROWED").length;
        elements.statOverdue.textContent = records.filter(item => getValue(item, "status") === "OVERDUE").length;
        elements.statReturned.textContent = records.filter(item => getValue(item, "status") === "RETURNED").length;
    }

    elements.logoutButton.addEventListener("click", () => { clearSession(); window.location.href = "login.html"; });
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
        if (!button) return;
        const action = button.dataset.action;
        const id = button.dataset.id;
        if (action === "edit") editRecord(id);
        if (action === "return") returnRecord(id);
        if (action === "delete") deleteRecord(id);
    });

    renderSession();
    renderPermissions();
    renderRecords();
    loadRecords(isAdminUser() ? "all" : "mine");

    // Init all other sections
    initBooksSection();
    initStudentsSection();
    initFinesSection();
}

// =====================================================================
//  BOOKS — non-admin can browse & search; admin has full CRUD
// =====================================================================
function initBooksSection() {
    const msg = document.getElementById("book-message");
    const tbody = document.getElementById("books-body");
    const count = document.getElementById("book-count");
    const form = document.getElementById("book-form");
    const newLink = document.getElementById("new-book-link");
    const cancelBtn = document.getElementById("cancel-book-edit");
    const saveBtn = document.getElementById("save-book-btn");
    const searchInput = document.getElementById("book-search");
    const searchBtn = document.getElementById("book-search-btn");
    const loadAllBtn = document.getElementById("load-all-books");

    // Hide admin-only UI for non-admins
    if (!isAdminUser()) {
        newLink.classList.add("hidden");
        form.classList.add("hidden");
    }

    function setMsg(text, isError = false) { setStatus(msg, text, isError); }

    async function loadBooks(searchName) {
        setMsg("Loading books...");
        try {
            let path = "/api/Book";
            if (searchName && searchName.trim()) {
                path = `/api/Book/name/${encodeURIComponent(searchName.trim())}`;
            }
            const data = await apiRequest(path);
            state.books = Array.isArray(data) ? data : [data].filter(Boolean);
            renderBooks();
            setMsg(`${state.books.length} book(s) loaded.`);
        } catch (error) {
            setMsg(error.message, true);
        }
    }

function renderBooks() {
        count.textContent = `${state.books.length} shown`;
        if (state.books.length === 0) {
            tbody.innerHTML = '<tr><td colspan="8" class="empty-cell">No books found.</td></tr>';
            return;
        }
        tbody.innerHTML = state.books.map(book => {
            const id = getValue(book, "bookId");
            const availableCopies = getValue(book, "availableCopies") || 0;
            let actions;
            if (isAdminUser()) {
                actions = `<button class="table-button" type="button" data-book-action="edit" data-book-id="${id}">Edit</button>
                           <button class="table-button" type="button" data-book-action="delete" data-book-id="${id}">Delete</button>`;
            } else if (availableCopies > 0) {
                actions = `<button class="table-button" type="button" data-book-action="borrow" data-book-id="${id}">Borrow</button>`;
            } else {
                actions = '<span class="muted-text">Not available</span>';
            }
            return `<tr>
                <td>${id}</td>
                <td>${escapeHtml(getValue(book, "title") || "")}</td>
                <td>${escapeHtml(getValue(book, "author") || "")}</td>
                <td>${escapeHtml(getValue(book, "isbn") || "")}</td>
                <td>${escapeHtml(getValue(book, "category") || "")}</td>
                <td>${getValue(book, "publishedYear") || "-"}</td>
                <td>${availableCopies}</td>
                <td><div class="row-actions">${actions}</div></td>
            </tr>`;
}).join("");
    }

async function borrowBook(bookId) {
        if (!bookId || bookId <= 0) { setMsg("Invalid book ID.", true); return; }
        setMsg("Borrowing book...");
        try {
            await apiRequest(`/api/BorrowRecord/borrow/${bookId}`, { method: "POST" });
            await loadBooks();
            setMsg("Book borrowed successfully! Check 'My Records' to view your borrowed books.");
        } catch (error) {
            setMsg(error.message, true);
        }
    }

    function getBookFormData() {
        return {
            title: document.getElementById("book-title").value.trim(),
            author: document.getElementById("book-author").value.trim(),
            isbn: document.getElementById("book-isbn").value.trim(),
            category: document.getElementById("book-category").value.trim(),
            publishedYear: document.getElementById("book-year").value ? Number(document.getElementById("book-year").value) : null,
            availableCopies: Number(document.getElementById("book-copies").value) || 1
        };
    }

    function resetBookForm() {
        state.bookEditingId = null;
        form.classList.add("hidden");
        document.getElementById("book-form-title").textContent = "Create Book";
        document.getElementById("book-editing-label").textContent = "New";
        document.getElementById("book-title").value = "";
        document.getElementById("book-author").value = "";
        document.getElementById("book-isbn").value = "";
        document.getElementById("book-category").value = "";
        document.getElementById("book-year").value = "";
        document.getElementById("book-copies").value = "1";
    }

    function fillBookForm(book) {
        document.getElementById("book-title").value = getValue(book, "title") || "";
        document.getElementById("book-author").value = getValue(book, "author") || "";
        document.getElementById("book-isbn").value = getValue(book, "isbn") || "";
        document.getElementById("book-category").value = getValue(book, "category") || "";
        document.getElementById("book-year").value = getValue(book, "publishedYear") || "";
        document.getElementById("book-copies").value = getValue(book, "availableCopies") ?? "1";
    }

    newLink.addEventListener("click", event => {
        event.preventDefault();
        if (!isAdminUser()) { setMsg("Only admins can create books.", true); return; }
        resetBookForm();
        form.classList.remove("hidden");
        form.scrollIntoView({ behavior: "smooth", block: "start" });
    });

    cancelBtn.addEventListener("click", resetBookForm);

    saveBtn.addEventListener("click", async () => {
        if (!isAdminUser()) { setMsg("Only admins can modify books.", true); return; }
        const payload = getBookFormData();
        if (!payload.title || !payload.author) {
            setMsg("Title and author are required.", true);
            return;
        }
        const isEditing = state.bookEditingId !== null;
        const path = isEditing ? `/api/Book/${state.bookEditingId}` : "/api/Book";
        setMsg(isEditing ? "Updating book..." : "Creating book...");
        setButtonLoading(saveBtn, true, isEditing ? "Updating..." : "Saving...");
        try {
            await apiRequest(path, { method: isEditing ? "PUT" : "POST", body: JSON.stringify(payload) });
            resetBookForm();
            await loadBooks();
            setMsg(isEditing ? "Book updated." : "Book created.");
        } catch (error) {
            setMsg(error.message, true);
        } finally {
            setButtonLoading(saveBtn, false);
        }
    });

tbody.addEventListener("click", event => {
        const btn = event.target.closest("button[data-book-action]");
        if (!btn) return;
        const action = btn.dataset.bookAction;
        const id = Number(btn.dataset.bookId);

        if (action === "borrow") {
            if (!isSignedIn()) { window.location.href = "login.html"; return; }
            if (isAdminUser()) { setMsg("Admin users must use the borrow record form to lend books.", true); return; }
            borrowBook(id);
        }

        if (action === "edit") {
            if (!isAdminUser()) { setMsg("Only admins can edit books.", true); return; }
            const book = state.books.find(b => Number(getValue(b, "bookId")) === id);
            if (!book) return;
            state.bookEditingId = id;
            document.getElementById("book-form-title").textContent = "Update Book";
            document.getElementById("book-editing-label").textContent = `#${id}`;
            fillBookForm(book);
            form.classList.remove("hidden");
            form.scrollIntoView({ behavior: "smooth", block: "start" });
        }

        if (action === "delete") {
            if (!isAdminUser()) { setMsg("Only admins can delete books.", true); return; }
            if (!confirm(`Delete book #${id}?`)) return;
            setMsg("Deleting book...");
            apiRequest(`/api/Book/${id}`, { method: "DELETE" })
                .then(() => loadBooks())
                .then(() => setMsg("Book deleted."))
                .catch(err => setMsg(err.message, true));
        }
    });

    searchBtn.addEventListener("click", () => loadBooks(searchInput.value));
    searchInput.addEventListener("keydown", e => { if (e.key === "Enter") loadBooks(searchInput.value); });
    loadAllBtn.addEventListener("click", () => { searchInput.value = ""; loadBooks(); });
}

// =====================================================================
//  STUDENTS — admin only
// =====================================================================
function initStudentsSection() {
    const msg = document.getElementById("student-message");
    const tbody = document.getElementById("students-body");
    const count = document.getElementById("student-count");
    const loadBtn = document.getElementById("load-all-students");

    function setMsg(text, isError = false) { setStatus(msg, text, isError); }

    async function loadStudents() {
        setMsg("Loading students...");
        try {
            const data = await apiRequest("/api/Student");
            state.students = Array.isArray(data) ? data : [];
            renderStudents();
            setMsg(`${state.students.length} student(s) loaded.`);
        } catch (error) {
            setMsg(error.message, true);
        }
    }

    function renderStudents() {
        count.textContent = `${state.students.length} shown`;
        if (state.students.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="empty-cell">No students found.</td></tr>';
            return;
        }
        tbody.innerHTML = state.students.map(s => `<tr>
            <td>${escapeHtml(getValue(s, "userId") || "")}</td>
            <td>${escapeHtml(getValue(s, "email") || "")}</td>
            <td>${escapeHtml(getValue(s, "section") || "")}</td>
            <td>${escapeHtml(getValue(s, "yearLevel") || "")}</td>
            <td>${normalizeDate(getValue(s, "createdAt"))}</td>
            <td>${getValue(s, "isActive") ? "Yes" : "No"}</td>
        </tr>`).join("");
    }

    loadBtn.addEventListener("click", loadStudents);
}

// =====================================================================
//  FINES — admin sees all; non-admin sees their own fines
// =====================================================================
function initFinesSection() {
    const msg = document.getElementById("fine-message");
    const tbody = document.getElementById("fines-body");
    const count = document.getElementById("fine-count");
    const loadBtn = document.getElementById("load-all-fines");

    function setMsg(text, isError = false) { setStatus(msg, text, isError); }

    async function loadFines() {
        setMsg("Loading fines...");
        try {
            let path = "/api/Fine";
            if (!isAdminUser()) {
                const uid = getUserId();
                if (!uid) throw new Error("Current user ID is missing.");
                path = `/api/Fine/user/${encodeURIComponent(uid)}`;
            }
            const data = await apiRequest(path);
            state.fines = Array.isArray(data) ? data : [];
            renderFines();
            setMsg(`${state.fines.length} fine(s) loaded.`);
        } catch (error) {
            setMsg(error.message, true);
        }
    }

    function renderFines() {
        count.textContent = `${state.fines.length} shown`;
        if (state.fines.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="empty-cell">No fines found.</td></tr>';
            return;
        }
        tbody.innerHTML = state.fines.map(f => {
            const status = String(getValue(f, "status") || "PENDING");
            return `<tr>
                <td>${getValue(f, "recordId") || "-"}</td>
                <td>${escapeHtml(getValue(f, "userId") || "")}</td>
                <td>$${Number(getValue(f, "amount") || 0).toFixed(2)}</td>
                <td><span class="status ${status.toLowerCase()}">${escapeHtml(status)}</span></td>
                <td>${normalizeDate(getValue(f, "paidAt"))}</td>
                <td>${normalizeDate(getValue(f, "createdAt"))}</td>
                <td><span class="muted-text">View only</span></td>
            </tr>`;
        }).join("");
    }

    loadBtn.addEventListener("click", loadFines);
}

// =====================================================================
//  INIT
// =====================================================================
const page = document.body.dataset.page;
if (page === "login") initLoginPage();
if (page === "signup") initSignupPage();
if (page === "dashboard") initDashboardPage();
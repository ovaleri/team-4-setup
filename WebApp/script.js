class AccountManager {
    constructor() {
        this.accounts = JSON.parse(localStorage.getItem('accounts')) || [];
        this.form = document.getElementById('accountForm');
        this.accountsList = document.getElementById('accountsList');
        this.submitBtn = document.getElementById('submitBtn');
        this.cancelBtn = document.getElementById('cancelBtn');
        
        this.initializeEventListeners();
        this.renderAccounts();
    }

    initializeEventListeners() {
        this.form.addEventListener('submit', (e) => this.handleSubmit(e));
        this.cancelBtn.addEventListener('click', () => this.resetForm());
    }

    handleSubmit(e) {
        e.preventDefault();
        
        const accountData = {
            username: document.getElementById('username').value,
            email: document.getElementById('email').value,
            role: document.getElementById('role').value
        };

        const accountId = document.getElementById('accountId').value;

        if (accountId) {
            this.updateAccount(accountId, accountData);
        } else {
            this.addAccount(accountData);
        }

        this.resetForm();
        this.renderAccounts();
    }

    addAccount(accountData) {
        accountData.id = Date.now().toString();
        this.accounts.push(accountData);
        this.saveAccounts();
    }

    updateAccount(accountId, accountData) {
        const index = this.accounts.findIndex(account => account.id === accountId);
        if (index !== -1) {
            this.accounts[index] = { ...accountData, id: accountId };
            this.saveAccounts();
        }
    }

    deleteAccount(accountId) {
        this.accounts = this.accounts.filter(account => account.id !== accountId);
        this.saveAccounts();
        this.renderAccounts();
    }

    editAccount(account) {
        document.getElementById('accountId').value = account.id;
        document.getElementById('username').value = account.username;
        document.getElementById('email').value = account.email;
        document.getElementById('role').value = account.role;
        
        this.submitBtn.textContent = 'Update Account';
        this.cancelBtn.style.display = 'inline-block';
    }

    resetForm() {
        document.getElementById('accountId').value = '';
        this.form.reset();
        this.submitBtn.textContent = 'Add Account';
        this.cancelBtn.style.display = 'none';
    }

    renderAccounts() {
        this.accountsList.innerHTML = this.accounts.map(account => `
            <div class="account-card">
                <div class="account-info">
                    <strong>${account.username}</strong><br>
                    ${account.email}<br>
                    <small>Role: ${account.role}</small>
                </div>
                <div class="account-actions">
                    <button class="edit-btn" onclick='accountManager.editAccount(${JSON.stringify(account)})'>Edit</button>
                    <button class="delete-btn" onclick='accountManager.deleteAccount("${account.id}")'>Delete</button>
                </div>
            </div>
        `).join('');
    }

    saveAccounts() {
        localStorage.setItem('accounts', JSON.stringify(this.accounts));
    }
}

// Initialize the account manager
const accountManager = new AccountManager();

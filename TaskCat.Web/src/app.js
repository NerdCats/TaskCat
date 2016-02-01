export class Welcome {
  heading = 'TaskCat Login';
  username = '';
  password = '';

  get fullName() {
    return `${this.username} ${this.password}`;
  }

  submit() {
    if(this.username && this.password){
      alert(`Welcome, ${this.username}!`);
    }
    else {
      alert("All the fields are required");
    }
  }
}

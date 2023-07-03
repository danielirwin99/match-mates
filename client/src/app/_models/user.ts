// Giving types to our API responses
export interface User {
  username: string;
  token: string;
  photoUrl: string;
  knownAs: string;
  gender: string;
  // The roles are contained inside an array
  roles: string[];
}

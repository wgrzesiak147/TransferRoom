export interface Player {
  playerId: number;
  firstName: string;
  lastName: string;
  position: string;
  dateOfBirth: string | null; // ISO date string or null
  photo: string;
}

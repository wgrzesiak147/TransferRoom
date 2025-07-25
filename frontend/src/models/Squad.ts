import type { Player } from "./Player";

export interface Squad {
  teamId: number;
  teamName: string;
  players: Player[];
}

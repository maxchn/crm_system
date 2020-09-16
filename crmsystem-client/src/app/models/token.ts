export class Token {
    constructor(public access_token: string,
        public access_token_type: string,
        public expires: string) { }
}
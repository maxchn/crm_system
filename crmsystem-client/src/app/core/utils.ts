export class Utils {
    public static API_BASE_URL: string = "http://localhost:44761/api";
    public static BASE_URL: string = "http://localhost:44761";
    public static CLIENT_URL: string = "http://localhost:4200";

    public static ACCESS_TOKEN_TYPE: string = 'access_token_type';
    public static ACCESS_TOKEN: string = 'access_token';
    public static EXPIRES: string = 'expires';
    public static USER_ID: string = 'user_id';
    public static COMPANY_ID: string = 'company_id';

    public static printInfoMessageToConsole(header: string) {
        console.log("====================================================");
        console.log(header);
        console.log("====================================================");
    }

    public static printValueToConsole(value: any) {
        console.dir(value);
        console.log("-----------------------------------------------------");
    }

    public static printValueWithHeaderToConsole(header: string, value: any) {
        console.log("====================================================");
        console.log(header);
        console.log("====================================================");
        console.dir(value);
        console.log("-----------------------------------------------------");
    }

    public static formatDate(date: any): string {
        try {
            let formatDate = new Date(date);
            return formatDate.toLocaleDateString();
        }
        catch (err) {
            Utils.printValueWithHeaderToConsole("[formatDate][error]", err);
            return date;
        }
    }

    public static formatDateTime(date: any): string {
        try {
            let formatDate = new Date(date);
            return formatDate.toLocaleString();
        }
        catch (err) {
            Utils.printValueWithHeaderToConsole("[formatDate][error]", err);
            return date;
        }
    }
}